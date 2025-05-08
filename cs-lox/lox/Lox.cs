class Lox {
  static void Main(string[] args) {
    if (args.Length > 1) {
      ReportTooManyArgsError();
    } else if (args.Length == 1) {
      RunFile(args[0]);
    } else {
      RunPrompt();
    };
  }

  static void ReportTooManyArgsError() {}

  static void RunFile(string path) {}

  static void RunPrompt() {}
}
