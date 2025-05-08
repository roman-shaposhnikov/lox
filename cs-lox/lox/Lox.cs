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

  static void ReportTooManyArgsError() {
    Console.WriteLine("Usage: cs-lox [script]");
    ExitWithCode(ExitCode.CLIArgsError);
  }

  static void RunFile(string path) {}

  static void RunPrompt() {}

  static void ExitWithCode(ExitCode exitCode) {
    Environment.Exit((int)exitCode);
  }
}

enum ExitCode : int {
  CLIArgsError = 64,
}
