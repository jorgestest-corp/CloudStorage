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

class sharefileDemo
{
  private static Sharefile sharefile = new nsoftware.async.CloudStorage.Sharefile();
  private static Oauth oauth = new nsoftware.async.CloudStorage.Oauth();
  private static string currentFolderId = "";
  private static string subdomain = "";

  static async Task Main(string[] args)
  {
    /* NOTE: ShareFile requires a registered redirect URI which is https://
     *       and explicitly disallows localhost. A valid public domain with SSL enabled must
     *       be specific when creating your ClientId and ClientSecret values.
     *       
     *       Visit https://api.sharefile.com/apikeys to create your credentials.
     *       
     * IMPORTANT: 
     *      Because of the ShareFile restrictions detailed above, this demo
     *      makes use of an existing public URL (oauth.nsoftware.com).
     *      This should NOT be used in production. It is only used here to
     *      facilitate running of this demo. In a real application, you should
     *      register your own Redirect URI.
     *      
     *      For any questions, please contact support@nsoftware.com.
     */

    try
    {
      // Prompt for authentication information.
      Console.Write("Enter your account subdomain: ");
      subdomain = Console.ReadLine();
      oauth.ClientProfile = OauthClientProfiles.ocpApplication;
      Console.Write("Enter your OAuth client ID: "); // Can use Hb7kR4d6mBvQxnOe9Eg6V8CMIVjQ67vD for testing purposes.
      oauth.ClientId = Console.ReadLine();
      Console.Write("Enter your OAuth client secret: "); // Can use jHAZH6f1Ac5O9krW9az3fLYFEZW4KoF1sr1hqDoVgrvv1zwc for testing purposes.
      oauth.ClientSecret = Console.ReadLine();

      oauth.ServerAuthURL = "https://secure.sharefile.com/oauth/authorize";
      oauth.ServerTokenURL = "https://" + subdomain + ".sharefile.com/oauth/token";

      oauth.WebServerPort = 7878; // Specify a particular port, as it will be used in the redirect.
      await oauth.StartWebServer(); // In most cases, the webserver is automatically started when GetAuthorization is called. In this case, due to the use of oauth.nsoftware.com, it is started manually.
      oauth.ReturnURL = "https://oauth.nsoftware.com/oauthdemo"; // This URL should NOT be used in a production application. It is only for testing purposes.
      await oauth.AddParam("state", "http://localhost:" + oauth.WebServerPort); // The oauth.nsoftware.com server uses this value to redirect to the local server embedded within the OAuth class.

      // oauth.RefreshToken = ""; // Set to value from previous runs to expedite the login process.
      sharefile.Authorization = await oauth.GetAuthorization();
      // Console.WriteLine(oauth.RefreshToken); // RefreshToken may be used to automatically obtain a new token without user interaction.

      // Process user commands.
      Console.WriteLine("Type \"?\" or \"help\" for a list of commands.");
      Console.Write("sharefile> ");
      string command;
      string[] arguments;

      Console.WriteLine("Home Directory Listings: ");
      await ListItems("home");

      while (true)
      {
        command = Console.ReadLine();
        arguments = command.Split();

        if (arguments[0] == "?" || arguments[0] == "help")
        {
          Console.WriteLine("Commands: ");
          Console.WriteLine("  ?                            display the list of valid commands");
          Console.WriteLine("  help                         display the list of valid commands");
          Console.WriteLine("  ls                           list items in the current folder");
          Console.WriteLine("  cd <id>                      change to the folder with the specified id");
          Console.WriteLine("  get <id>                     download the item with the specified id");
          Console.WriteLine("  put <file>                   upload a new file");
          Console.WriteLine("  del <id>                     delete the item with the specified id");
          Console.WriteLine("  mkdir <name>                 make a new folder");
          Console.WriteLine("  share <id>                   share the item with the specified id");
          Console.WriteLine("  request                      create a new request link");
          Console.WriteLine("  quit                         exit the application");
        }
        else if (arguments[0] == "ls")
        {
          await ListItems(currentFolderId);
        }
        else if (arguments[0] == "cd")
        {
          if (arguments.Length > 1)
          {
            int folderSelection = int.Parse(arguments[1]);
            if (folderSelection >= 0)
            {
              await ListItems(sharefile.Items[folderSelection].Id);
            }
            else
            {
              await sharefile.GetItemInfo(currentFolderId);
              await ListItems(sharefile.Items[0].ParentId);
            }
          }
        }
        else if (arguments[0] == "get")
        {
          if (arguments.Length > 1)
          {
            ShareFileItem downloadSelection = sharefile.Items[int.Parse(arguments[1])];
            sharefile.LocalFile = downloadSelection.Name;
            await sharefile.DownloadFile(downloadSelection.Id);

            Console.WriteLine("Download complete.");
          }
        }
        else if (arguments[0] == "put")
        {
          if (arguments.Length > 1)
          {
            sharefile.LocalFile = arguments[1];
            FileInfo file = new FileInfo(sharefile.LocalFile);
            await sharefile.UploadFile(file.Name, currentFolderId);

            Console.WriteLine("Upload complete.");
          }
        }
        else if (arguments[0] == "del")
        {
          if (arguments.Length > 1)
          {
            ShareFileItem deleteSelection = sharefile.Items[int.Parse(arguments[1])];
            await sharefile.DeleteItem(deleteSelection.Id);

            Console.WriteLine("Deletion complete.");
          }
        }
        else if (arguments[0] == "mkdir")
        {
          if (arguments.Length > 1)
          {
            await sharefile.CreateFolder(arguments[1], currentFolderId);
          }
        }
        else if (arguments[0] == "share")
        {
          if (arguments.Length > 1)
          {
            ShareFileItem shareSelection = sharefile.Items[int.Parse(arguments[1])];
            Console.WriteLine("Share Link: " + await sharefile.CreateLink(shareSelection.Id));
          }
        }
        else if (arguments[0] == "request")
        {
          await sharefile.CreateRequestLink(currentFolderId);
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

        Console.Write("sharefile> ");
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
    }
  }

  private static async Task ListItems(string folderId)
  {
    try
    {
      sharefile.AccountSubdomain = subdomain;
      await sharefile.GetItemInfo(folderId);
      currentFolderId = sharefile.Items[0].Id;

      Console.WriteLine();
      Console.WriteLine("{0,-3}{1,-40}{2,-20}{3,-30}", "#", "Title", "Type", "Creation Date");
      Console.WriteLine("--------------------------------------------------------------------------------");

      await sharefile.ListItems(currentFolderId);
      Console.WriteLine("{0,-3}{1,-20}{2,-30}{3,-40}", "-1", "..", "Folder", "");

      for (int i = 0; i < sharefile.Items.Count; i++)
      {
        ShareFileItem currentItem = sharefile.Items[i];
        Console.WriteLine("{0,-3}{1,-20}{2,-30}{3,-40}", i, sharefile.Items[i].Name, ResolveResourceType(sharefile.Items[i].Type), sharefile.Items[i].CreationDate);
      }
      Console.WriteLine();
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
    }
  }

  private static string ResolveResourceType(ShareFileItemTypes type)
  {
    switch (type)
    {
      case ShareFileItemTypes.sfitFile:
        return "File";
      case ShareFileItemTypes.sfitFolder:
        return "Folder";
      default:
        return "Unknown";
    }
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