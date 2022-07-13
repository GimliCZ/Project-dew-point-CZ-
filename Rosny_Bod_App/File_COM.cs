using System;
using System.IO;
using System.Text;

namespace Rosny_Bod_App
{
    public class File_COM
    {
        public string current_path = Directory.GetCurrentDirectory();

        public void Add_to(string filename, string foldername, string data, bool Dont_Clear_file) //Zapiš do souboru data, ( s možností přepisu )
        {
            string temp = current_path + "\\" + foldername + "\\" + filename;
            try
            {
                using (StreamWriter write = new StreamWriter(temp, Dont_Clear_file, Encoding.GetEncoding(1252)))
                {
                    write.WriteLine(data);
                }
            }
            catch (FileNotFoundException)
            {
                using (StreamWriter write = File.CreateText(filename))
                {
                    write.WriteLine(data);
                }
            }
        }

        /*  public void create_file(string filename,string foldername)
          {
              try
              {
                  using (FileStream fs = File.Create(current_path + "//" + foldername + "//" + filename)) ;
              }
              catch(Exception)
              {
              }
          }*/

        public void create_folder(string foldername) //Založ složku
        {
            try
            {
                Directory.CreateDirectory(current_path + "//" + foldername);
            }
            catch (Exception)
            {
            }
        }
    }
}