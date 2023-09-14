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
using System.Threading.Tasks;
using nsoftware.async.CloudStorage;

class wasabiDemo
{
  private static Wasabi wasabi = new nsoftware.async.CloudStorage.Wasabi();

  static async Task Main(string[] args)
  {
    try
    {
      wasabi.OnBucketList += wasabi_OnBucketList;
      wasabi.OnObjectList += wasabi_OnObjectList;
      wasabi.OnSSLServerAuthentication += wasabi_OnSSLServerAuthentication;

      // Prompt for authentication information.
      Console.Write("Enter your access key: ");
      wasabi.AccessKey = Console.ReadLine();
      Console.Write("Enter your secret key: ");
      wasabi.SecretKey = Console.ReadLine();

      // Process user commands.
      Console.WriteLine("Type \"?\" or \"help\" for a list of commands.");
      Console.Write("wasabi> ");
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
          Console.WriteLine("  cd <bucket>                  change to the specified bucket");
          Console.WriteLine("  lb                           list all buckets");
          Console.WriteLine("  lo                           list all objects in the currently selected bucket");
          Console.WriteLine("  get <object>                 get the specified object");
          Console.WriteLine("  put <name> <file>            create a new object in the currently selected bucket");
          Console.WriteLine("  quit                         exit the application");
        }
        else if (arguments[0] == "cd")
        {
          if (arguments.Length > 1)
          {
            wasabi.Bucket = arguments[1];
          }
        }
        else if (arguments[0] == "lb")
        {
          await wasabi.ListBuckets();
        }
        else if (arguments[0] == "lo")
        {
          await wasabi.ListObjects();
        }
        else if (arguments[0] == "get")
        {
          if (arguments.Length > 1)
          {
            await wasabi.GetObject(arguments[1]);
            Console.WriteLine("Content of the object:\n" + wasabi.ObjectData);
          }
        }
        else if (arguments[0] == "put")
        {
          if (arguments.Length > 2)
          {
            wasabi.LocalFile = arguments[2];
            await wasabi.CreateObject(arguments[1]);
            Console.WriteLine("Object created.");
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

        Console.Write("wasabi> ");
      }
    }
    catch (Exception e)
    {
      Console.WriteLine(e.Message);
    }
  }

  private static void wasabi_OnBucketList(object sender, WasabiBucketListEventArgs e)
  {
    Console.WriteLine(e.BucketName);
  }

  private static void wasabi_OnObjectList(object sender, WasabiObjectListEventArgs e)
  {
    Console.WriteLine(e.ObjectName);
  }

  private static void wasabi_OnSSLServerAuthentication(object sender, WasabiSSLServerAuthenticationEventArgs e)
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