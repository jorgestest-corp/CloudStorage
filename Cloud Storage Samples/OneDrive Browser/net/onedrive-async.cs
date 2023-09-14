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

class onedriveDemo
{
  private static Onedrive onedrive = new nsoftware.async.CloudStorage.Onedrive();

  static async Task Main(string[] args)
  {
    try
    {
      onedrive.OnSSLServerAuthentication += onedrive_OnSSLServerAuthentication;

      // Prompt for authentication information.
      Console.Write("Enter your OAuth client ID: ");
      onedrive.OAuth.ClientId = Console.ReadLine();
      Console.Write("Enter your OAuth client secret: ");
      onedrive.OAuth.ClientSecret = Console.ReadLine();

      onedrive.OAuth.ServerAuthURL = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize";
      onedrive.OAuth.ServerTokenURL = "https://login.microsoftonline.com/common/oauth2/v2.0/token";
      onedrive.OAuth.AuthorizationScope = "offline_access files.readwrite files.readwrite.all";

      Console.Write("Enter a redirect URI port: ");
      await onedrive.Config("OAuthWebServerPort=" + Console.ReadLine());
      await onedrive.Config("OAuthBrowserResponseTimeout=60");
      await onedrive.Authorize();

      // Process user commands.
      Console.WriteLine("Type \"?\" or \"help\" for a list of commands.");
      Console.Write("onedrive> ");
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
          Console.WriteLine("  ls                           list resources in the root folder");
          Console.WriteLine("  get <id>                     download the file resource with the specified id");
          Console.WriteLine("  put <file>                   upload a new file resource");
          Console.WriteLine("  del <id>                     delete the resource with the specified id");
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
            OneDriveResource resource = onedrive.Resources[int.Parse(arguments[1])];
            onedrive.LocalFile = resource.Name;
            onedrive.RemoteId = resource.Id;
            await onedrive.DownloadFile(); // Use the default file format.

            Console.WriteLine("Download complete.");
          }
        }
        else if (arguments[0] == "put")
        {
          if (arguments.Length > 1)
          {
            onedrive.LocalFile = arguments[1];
            onedrive.RemoteId = "";
            FileInfo file = new FileInfo(onedrive.LocalFile);
            await onedrive.UploadFile(file.Name);

            Console.WriteLine("Upload complete.");
          }
        }
        else if (arguments[0] == "del")
        {
          if (arguments.Length > 1)
          {
            OneDriveResource resource = onedrive.Resources[int.Parse(arguments[1])];
            onedrive.RemoteId = resource.Id;
            await onedrive.DeleteResource();

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

        Console.Write("onedrive> ");
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
      onedrive.RemoteId = ""; // Clear the documents collection.
      onedrive.RemotePath = "";
      await onedrive.ListResources();

      Console.WriteLine();
      Console.WriteLine("{0,-3}{1,-40}{2,-20}{3,-30}", "#", "Title", "Type", "Last Modified");
      Console.WriteLine("--------------------------------------------------------------------------------");

      for (int i = 0; i < onedrive.Resources.Count; i++)
      {
        Console.WriteLine("{0,-3}{1,-40}{2,-20}{3,-30}", i, onedrive.Resources[i].Name, ResolveResourceType(onedrive.Resources[i].Type), onedrive.Resources[i].ModifiedTime);
      }
      Console.WriteLine();
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
    }
  }

  private static string ResolveResourceType(OneDriveResourceTypes type)
  {
    switch (type)
    {
      case OneDriveResourceTypes.odrtFile:
        return "File";
      case OneDriveResourceTypes.odrtFolder:
        return "Folder";
      default:
        return "Unknown";
    }
  }

  private static void onedrive_OnSSLServerAuthentication(object sender, OnedriveSSLServerAuthenticationEventArgs e)
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