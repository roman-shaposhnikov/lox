/// String-aware line splitter for Lox source code.
///
/// Unlike `str::lines()`, this iterator understands string literals and keeps
/// multi-line strings together as a single logical line. Line numbers are tracked
/// correctly even when strings span multiple physical lines.
///
/// # Example
/// ```ignore
/// let source = "print \"hello\nworld\"\nvar x = 42;";
/// let mut lines = LoxLines::new(source);
///
/// // First logical line contains the entire multi-line string
/// assert_eq!(lines.next(), Some((1, "print \"hello\nworld\"")));
/// // Next line starts at line 3 (accounting for newline in string)
/// assert_eq!(lines.next(), Some((3, "var x = 42;")));
/// ```
pub struct LoxLines {
    remaining: &'static str,
    current_line: usize,
}

impl LoxLines {
    pub fn new(source: &'static str) -> Self {
        Self {
            remaining: source,
            current_line: 1,
        }
    }
}

impl Iterator for LoxLines {
    type Item = (usize, &'static str);

    fn next(&mut self) -> Option<Self::Item> {
        if self.remaining.is_empty() {
            return None;
        }

        let start_line = self.current_line;
        let mut in_string = false;
        let mut end_pos = self.remaining.len();
        let mut found_split = false;

        for (i, ch) in self.remaining.char_indices() {
            match ch {
                '"' => in_string = !in_string,
                '\n' => {
                    if in_string {
                        // Inside string: count line but keep going
                        self.current_line += 1;
                    } else {
                        // Outside string: split here
                        end_pos = i;
                        found_split = true;
                        break;
                    }
                }
                _ => {}
            }
        }

        let content = if found_split {
            let line = &self.remaining[..end_pos];
            self.remaining = &self.remaining[end_pos + 1..]; // skip the \n
            self.current_line += 1; // count the split line
            line
        } else {
            // Last line (no trailing newline)
            let line = self.remaining;
            self.remaining = "";
            line
        };

        Some((start_line, content))
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use rstest::rstest;

    #[rstest]
    #[case("", vec![])]
    #[case("var x = 42;", vec![(1, "var x = 42;")])]
    #[case("var x = 42;\nvar y = 13;", vec![(1, "var x = 42;"), (2, "var y = 13;")])]
    fn basic_splitting(#[case] input: &'static str, #[case] expected: Vec<(usize, &'static str)>) {
        let lines: Vec<_> = LoxLines::new(input).collect();
        assert_eq!(lines, expected);
    }

    #[rstest]
    #[case("print \"hello\nworld\";", vec![(1, "print \"hello\nworld\";")])]
    #[case("print \"hello\nworld\";\nvar x = 42;", vec![(1, "print \"hello\nworld\";"), (3, "var x = 42;")])]
    #[case("print \"a\nb\nc\";\nnext;", vec![(1, "print \"a\nb\nc\";"), (4, "next;")])]
    #[case("var x = \"hello\nworld", vec![(1, "var x = \"hello\nworld")])]
    fn actual_newlines_in_strings(#[case] input: &'static str, #[case] expected: Vec<(usize, &'static str)>) {
        let lines: Vec<_> = LoxLines::new(input).collect();
        assert_eq!(lines, expected);
    }

    #[rstest]
    #[case("var x = \"hello\\nworld\";", vec![(1, "var x = \"hello\\nworld\";")])]
    #[case("var x = \"a\\nb\\nc\";", vec![(1, "var x = \"a\\nb\\nc\";")])]
    #[case("print \"esc\\n\";\nvar y = 42;", vec![(1, "print \"esc\\n\";"), (2, "var y = 42;")])]
    fn escape_sequences_ignored(#[case] input: &'static str, #[case] expected: Vec<(usize, &'static str)>) {
        let lines: Vec<_> = LoxLines::new(input).collect();
        assert_eq!(lines, expected);
    }

    #[test]
    fn escape_vs_actual_newline_tracking() {
        let source = "print \"esc\\n\";\nvar x = \"real\nline\";\nvar y = 42;";
        let lines: Vec<_> = LoxLines::new(source).collect();
        assert_eq!(lines, vec![
            (1, "print \"esc\\n\";"),
            (2, "var x = \"real\nline\";"),
            (4, "var y = 42;")
        ]);
    }
}
