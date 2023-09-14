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

class cloudfilesDemo
{
  private static Cloudfiles cloudfiles;
  private static Oauth oauth;

  private static void cloudfiles_OnSSLServerAuthentication(object sender, CloudfilesSSLServerAuthenticationEventArgs e)
  {
    if (e.Accept) return;
    Console.Write("Server provided the following certificate:\nIssuer: " + e.CertIssuer + "\nSubject: " + e.CertSubject + "\n");
    Console.Write("The following problems have been determined for this certificate: " + e.Status + "\n");
    Console.Write("Would you like to continue anyways? [y/n] ");
    if (Console.Read() == 'y') e.Accept = true;
  }

  static async Task Main(string[] args)
  {
    cloudfiles = new Cloudfiles();
    oauth = new Oauth();

    try
    {
      cloudfiles.OnSSLServerAuthentication += cloudfiles_OnSSLServerAuthentication;

      Console.WriteLine("Which cloud service provider would you like to use to send your message?");
      Console.WriteLine("   [0] - Amazon S3");
      Console.WriteLine("   [1] - GoogleStorage");
      Console.WriteLine("   [2] - Azure Blob");

      int servicePrompt = int.Parse(Console.ReadLine());

      // Prompt for authentication information.
      if (servicePrompt == 0)
      {
        cloudfiles.ServiceProvider = CloudfilesServiceProviders.cspAmazonS3;

        Console.Write("Enter your Amazon access key:  ");
        cloudfiles.Account.AccessKey = Console.ReadLine();

        Console.Write("Enter your Amazon secret key:  ");
        cloudfiles.Account.SecretKey = Console.ReadLine();
      }
      else if (servicePrompt == 1)
      {
        cloudfiles.ServiceProvider = CloudfilesServiceProviders.cspGoogleStorage;
        oauth.ServerAuthURL = "https://accounts.google.com/o/oauth2/auth";
        oauth.ServerTokenURL = "https://accounts.google.com/o/oauth2/token";

        Console.Write("Enter your OAuth client ID:  ");
        oauth.ClientId = Console.ReadLine();

        Console.Write("Enter your OAuth client secret:  ");
        oauth.ClientSecret = Console.ReadLine();

        Console.Write("Enter desired authorization scopes (leave a space between each one if providing multiple):  ");
        oauth.AuthorizationScope = Console.ReadLine();

        Console.Write("Authenticating... ");
        //cloudfiles.Account.Authorization = await oauth.GetAuthorization();
        cloudfiles.Account.AuthMechanism = CSAuthMechanisms.camOAuth;
        cloudfiles.Authorization = await oauth.GetAuthorization();
        Console.WriteLine(" done.");
      }
      else if (servicePrompt == 2)
      {
        cloudfiles.ServiceProvider = CloudfilesServiceProviders.cspAzureBlob;
        oauth.ServerAuthURL = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize";
        oauth.ServerTokenURL = "https://login.microsoftonline.com/common/oauth2/v2.0/token";

        Console.Write("Enter your OAuth client ID:  ");
        oauth.ClientId = Console.ReadLine();

        Console.Write("Enter your OAuth client secret:  ");
        oauth.ClientSecret = Console.ReadLine();

        Console.Write("Authenticating... ");
        cloudfiles.Account.AuthMechanism = CSAuthMechanisms.camOAuth;
        cloudfiles.Authorization = await oauth.GetAuthorization();
        Console.WriteLine(" done.");
      }
      else
      {
        throw new Exception("\nInvalid cloud service provider.\n");
      }

      // Prompt for message information.
      Console.Write("What would you like to do?\n[0] ");
      Console.WriteLine("   [0] - Upload a file");
      Console.WriteLine("   [1] - Download a file");
      Console.WriteLine("   [2] - Delete a file");
      Console.WriteLine("   [3] - Create Directory");
      Console.WriteLine("   [4] - Delete Directory");
      Console.WriteLine("   [5] - Quit");

      servicePrompt = int.Parse(Console.ReadLine());

      string filename;
      FileStream mystream;

      while (servicePrompt != 5)
      {
        switch (servicePrompt)
        {
          case 0:
            Console.Write("File path: ");
            filename = Console.ReadLine();
            mystream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            cloudfiles.SetUploadStream(mystream);
            cloudfiles.RemoteFile = filename;
            cloudfiles.Upload();
            break;
          case 1:
            Console.Write("File path: ");
            filename = Console.ReadLine();
            mystream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            cloudfiles.SetDownloadStream(mystream);
            cloudfiles.RemoteFile = filename;
            cloudfiles.Download();
            break;
          case 2:
            Console.Write("File path: ");
            filename = Console.ReadLine();
            cloudfiles.DeleteFile(filename);
            break;
          case 3:
            Console.Write("Directory name: ");
            filename = Console.ReadLine();
            cloudfiles.MakeDirectory(filename);
            break;
          case 4:
            Console.Write("Directory name: ");
            filename = Console.ReadLine();
            cloudfiles.RemoveDirectory(filename);
            break;
        }
        Console.Write("What would you like to do?\n[0] ");
        Console.WriteLine("   [0] - Upload a file");
        Console.WriteLine("   [1] - Download a file");
        Console.WriteLine("   [2] - Delete a file");
        Console.WriteLine("   [3] - Create Directory");
        Console.WriteLine("   [4] - Delete Directory");
        Console.WriteLine("   [5] - Quit");

        servicePrompt = int.Parse(Console.ReadLine());
      }
    }
    catch (Exception e)
    {
      Console.WriteLine(e.Message);
    }

    Console.WriteLine("\nPress any key to exit...");
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