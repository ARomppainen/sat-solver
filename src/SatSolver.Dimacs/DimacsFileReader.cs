using SatSolver.Shared;

namespace SatSolver.Dimacs;

public class DimacsFileReader
{
    public static List<Formula> ReadFormulas(string filePath)
    {
        string path = Path.IsPathRooted(filePath)
            ? filePath
            : Path.GetRelativePath(Directory.GetCurrentDirectory(), filePath);

        if (File.Exists(path))
        {
            using StreamReader reader = File.OpenText(path);
            Formula formula = DimacsParser.Parse(path, reader.Lines());
            return [formula];
        }

        if (Directory.Exists(path))
        {
            List<Formula> formulas = [];

            foreach (string filepath in Directory.GetFiles(path, "*.cnf"))
            {
                using StreamReader reader = File.OpenText(filepath);
                Formula formula = DimacsParser.Parse(filepath, reader.Lines());
                formulas.Add(formula);
            }

            if (formulas.Count == 0)
            {
                throw new ArgumentException($"No .cnf files found in the directory: {path}");
            }

            return formulas;
        }

        throw new ArgumentException($"File or directory does not exist: {path}");
    }
}
