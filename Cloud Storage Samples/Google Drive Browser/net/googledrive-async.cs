/*
 * Cloud Storage 2022 .NET Edition - Sample Project
 *
 * This sample project demonstrates the usage of Cloud Storage in a 
 * simple, straightforward way. It is not intended to be a complete 
 * application. Error handling and other checks are simplified for clarity.
 *
 * www.nsoftware.com/cloudstorage
 *
 * This code is subject to the terms and conditions specified in the 
 * corresponding product license agreement which outlines the authorized 
 * usage and restrictions.
 * 
 */

using System.Collections.Generic;
﻿using System;
using System.IO;
using System.Threading.Tasks;
using nsoftware.async.CloudStorage;

class googledriveDemo
{
  private static Googledrive googledrive = new nsoftware.async.CloudStorage.Googledrive();

  static async Task Main(string[] args)
  {
    try
    {
      googledrive.OnSSLServerAuthentication += googledrive_OnSSLServerAuthentication;

      // Prompt for authentication information.
      Console.Write("Enter your OAuth client ID: ");
      googledrive.OAuth.ClientId = Console.ReadLine();
      Console.Write("Enter your OAuth client secret: ");
      googledrive.OAuth.ClientSecret = Console.ReadLine();

      googledrive.OAuth.ServerAuthURL = "https://accounts.google.com/o/oauth2/auth";
      googledrive.OAuth.ServerTokenURL = "https://accounts.google.com/o/oauth2/token";
      googledrive.OAuth.AuthorizationScope = "https://www.googleapis.com/auth/drive";

      await googledrive.Config("OAuthBrowserResponseTimeout=60");
      await googledrive.Authorize();

      // Process user commands.
      Console.WriteLine("Type \"?\" or \"help\" for a list of commands.");
      Console.Write("googledrive> ");
      string command;
      string[] arguments;

      while (true)
      {
        command = Console.ReadLine();
        arguments = command.Split();

        if (arguments[0] == "?" || arguments[0] == "help")
        {
          Console.WriteLine("Commands: ");
          Console.WriteLine("  ?                            display the list of valid commands");
          Console.WriteLine("  help                         display the list of valid commands");
          Console.WriteLine("  ls                           list files and folers in the \"My Drive\" and \"Shared with me\" spaces");
          Console.WriteLine("  get <id>                     download the file with the specified id");
          Console.WriteLine("  put <file>                   upload a new file");
          Console.WriteLine("  del <id>                     delete the file or folder with the specified id");
          Console.WriteLine("  quit                         exit the application");
        }
        else if (arguments[0] == "ls")
        {
          await ListDocuments();
        }
        else if (arguments[0] == "get")
        {
          if (arguments.Length > 1)
          {
            GDriveResource resource = googledrive.Resources[int.Parse(arguments[1])];
            googledrive.LocalFile = resource.Name;
            await googledrive.DownloadFile(resource.Id, ""); // Use the default file format.

            Console.WriteLine("Download complete.");
          }
        }
        else if (arguments[0] == "put")
        {
          if (arguments.Length > 1)
          {
            googledrive.LocalFile = arguments[1];
            FileInfo file = new FileInfo(googledrive.LocalFile);
            await googledrive.UploadFile(file.Name, "");

            Console.WriteLine("Upload complete.");
          }
        }
        else if (arguments[0] == "del")
        {
          if (arguments.Length > 1)
          {
            GDriveResource resource = googledrive.Resources[int.Parse(arguments[1])];
            await googledrive.DeleteResource(resource.Id);

            Console.WriteLine("Deletion complete.");
          }
        }
        else if (arguments[0] == "quit")
        {
          break;
        }
        else if (arguments[0] == "")
        {
          // Do nothing.
        }
        else
        {
          Console.WriteLine("Invalid command.");
        } // End of command checking.

        Console.Write("googledrive> ");
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
    }
  }

  private static async Task ListDocuments()
  {
    try
    {
      await googledrive.ListResources();

      Console.WriteLine();
      Console.WriteLine("{0,-3}{1,-40}{2,-20}{3,-30}", "#", "Name", "Author", "Last Modified");
      Console.WriteLine("--------------------------------------------------------------------------------");

      for (int i = 0; i < googledrive.Resources.Count; i++)
      {
        Console.WriteLine("{0,-3}{1,-40}{2,-20}{3,-30}", i, googledrive.Resources[i].Name, googledrive.Resources[i].Owner, googledrive.Resources[i].ModifiedTime);
      }
      Console.WriteLine();
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
    }
  }

  private static void googledrive_OnSSLServerAuthentication(object sender, GoogledriveSSLServerAuthenticationEventArgs e)
  {
    if (e.Accept) return;
    Console.Write("Server provided the following certificate:\nIssuer: " + e.CertIssuer + "\nSubject: " + e.CertSubject + "\n");
    Console.Write("The following problems have been determined for this certificate: " + e.Status + "\n");
    Console.Write("Would you like to continue anyways? [y/n] ");
    if (Console.Read() == 'y') e.Accept = true;
  }
}


class ConsoleDemo
{
  public static Dictionary<string, string> ParseArgs(string[] args)
  {
    Dictionary<string, string> dict = new Dictionary<string, string>();

    for (int i = 0; i < args.Length; i++)
    {
      // If it starts with a "/" check the next argument.
      // If the next argument does NOT start with a "/" then this is paired, and the next argument is the value.
      // Otherwise, the next argument starts with a "/" and the current argument is a switch.

      // If it doesn't start with a "/" then it's not paired and we assume it's a standalone argument.

      if (args[i].StartsWith("/"))
      {
        // Either a paired argument or a switch.
        if (i + 1 < args.Length && !args[i + 1].StartsWith("/"))
        {
          // Paired argument.
          dict.Add(args[i].TrimStart('/'), args[i + 1]);
          // Skip the value in the next iteration.
          i++;
        }
        else
        {
          // Switch, no value.
          dict.Add(args[i].TrimStart('/'), "");
        }
      }
      else
      {
        // Standalone argument. The argument is the value, use the index as a key.
        dict.Add(i.ToString(), args[i]);
      }
    }
    return dict;
  }

  public static string Prompt(string prompt, string defaultVal)
  {
    Console.Write(prompt + (defaultVal.Length > 0 ? " [" + defaultVal + "]": "") + ": ");
    string val = Console.ReadLine();
    if (val.Length == 0) val = defaultVal;
    return val;
  }
}