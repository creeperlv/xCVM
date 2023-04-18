namespace xCVM.ShellUtilities
{
    public static class Shell
    {
        public static void Say(Sentence sentence)
        {
            for (int i = 0; i < sentence.PaddingTop; i++)
            {
                Console.WriteLine();
            }
            for (int i = 0; i < sentence.Intend; i++)
            {
                Console.Write("\t");
            }
            foreach (var item in sentence.Words)
            {
                if (item.Color == null)
                {
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = item.Color.Value;
                }
                Console.Write(item.Content);
            }
            if (sentence.EndWithNewLine)
            {
                Console.WriteLine();
            }
            for (int i = 0; i < sentence.PaddingBottom; i++)
            {
                Console.WriteLine();
            }
        }
    }
    public class Sentence
    {
        public int PaddingTop = 0;
        public int PaddingBottom = 0;
        public int Intend = 0;
        public List<Term> Words = new List<Term>();
        public bool EndWithNewLine;
    }
    public class Term
    {
        public ConsoleColor? Color = null;
        public string Content = "";
    }

}