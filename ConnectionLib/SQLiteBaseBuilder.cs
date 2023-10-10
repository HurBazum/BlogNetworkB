namespace ConnectionLib
{
    public static class SQLiteBaseBuilder
    {
        public static string GetConnectionString(AppDomain appDomain)
        {
            string folderPath = appDomain.BaseDirectory + "/BlogNetworkDatabase.db";

            if(!File.Exists(folderPath))
            {
                File.Create(folderPath);
            }

            return "Data Source=" + folderPath;
        }
    }
}