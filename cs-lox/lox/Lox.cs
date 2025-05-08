class Lox {
  static public Boolean hadError = false;
  static public Boolean hadRuntimeError = false;

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

  static void RunFile(string path) {
    string source = File.ReadAllText(path);
    Run(source);

    if (hadError) {
      ExitWithCode(ExitCode.DataFormatError);
    }
    if (hadRuntimeError) {
      ExitWithCode(ExitCode.InternalProgramError);
    }
  }

  static void RunPrompt() {
    while (true) {
      string? line  = Console.ReadLine();
      if (line == null) {
        break;
      }

      Run(line);
      hadError = false;
    }
  }

  static void ExitWithCode(ExitCode exitCode) {
    Environment.Exit((int)exitCode);
  }

  static void Run(string source) {
    var scanner = new Scanner(source);
    var tokens = scanner.ProduceTokens();

    tokens.ForEach((token) => {
      Console.WriteLine(token);
    });
  }
}

enum ExitCode : int {
  CLIArgsError = 64,
  DataFormatError = 65,
  InternalProgramError = 70,
}
