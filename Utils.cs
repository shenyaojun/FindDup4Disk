namespace WinFormsApp1
{
    internal class Utils
    {
        public static Dictionary<string, List<string>> CsvToDict(string path, int keyIndex = 0, string commentSign = "#")
        {
            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();

            StreamReader sr = new StreamReader(path);
            string content = sr.ReadToEnd();
            sr.Close();

            string[] contents = content.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var c in contents)
            {
                if (!c.StartsWith(commentSign))
                {
                    string[] cc = c.Split(',');
                    string key = string.Empty;
                    List<string> values = new List<string>();
                    for (var i = 0; i < cc.Length; i++)
                    {

                        if (i == keyIndex)
                        {
                            key = cc[i];
                        }
                        else
                        {
                            values.Add(cc[i]);
                        }
                    }
                    dict.Add(key, values);
                }
            }
            return dict;
        }
    }
}
