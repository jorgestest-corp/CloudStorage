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
using nsoftware.async.CloudStorage;
using System;
using System.Threading.Tasks;

public class Amazons3Demo
{
  // Declare private static fields for Amazons3 object and currentBucket string.
  private static Amazons3 amazons3;
  private static string currentBucket;

  // Define Main function as an asynchronous task.
  static async Task Main(string[] args)
  {
    try
    {
      //Prompt the user to enter AWS access key and secret key.
      Console.Write("AWS Access Key: ");
      var accessKey = Console.ReadLine();

      Console.Write("AWS Secret Key: ");
      var secretKey = Console.ReadLine();

      // Initialize the Amazons3 object and set its access and secret keys.
      amazons3 = new Amazons3();
      amazons3.AccessKey = accessKey;
      amazons3.SecretKey = secretKey;


      // Print bucket list title and then list all the buckets.
      Console.WriteLine("\nBucket List:\n");
      await amazons3.ListBuckets();
      foreach (var bucket in amazons3.Buckets)
      {
        Console.WriteLine(bucket.Name);
      }

      // Print the menu options to the console.
      PrintMenu();

      // Loop until user enters "q" for quit.
      string buffer = "";
      while (!buffer.Equals("q", StringComparison.InvariantCultureIgnoreCase))
      {
        Console.Write("Enter command: ");
        buffer = Console.ReadLine();

        // Switch statement to handle different commands entered by the user.
        switch (buffer.ToLower().Trim())
        {
          case "cd":
            // Change the current bucket.
            Console.Write("Which bucket?: ");
            amazons3.Bucket = Console.ReadLine();
            Console.WriteLine($"Current bucket is now {amazons3.Bucket}.");
            break;

          case "ls":
            // List all objects in the current bucket.
            try
            {
              await amazons3.ListObjects();
            }
            catch (CloudStorageAmazons3Exception ex)
            {
              Console.WriteLine($"{ex.Message}");
              break;
            }
            foreach (var entry in amazons3.Objects)
            {
              Console.WriteLine(entry.Name);
            }
            break;

          case "get":
            // Get an object (file) from the current bucket.
            Console.Write("Which file: ");
            var key = Console.ReadLine();
            await amazons3.GetObject(key);
            Console.WriteLine("The content of the file:\n=========================\n" + amazons3.ObjectData + "\n=========================\n");
            break;

          case "put":
            // Upload a local file to the current bucket as a new object.
            Console.Write("Name of new object: ");
            var newObject = Console.ReadLine();
            Console.Write("Local file to upload: ");
            amazons3.LocalFile = Console.ReadLine();
            await amazons3.CreateObject(newObject);
            Console.WriteLine("Object created.");
            break;

          case "q":
            // Exit the program.
            Environment.Exit(0);
            break;

          case "?":
            // Print the menu again.
            PrintMenu();
            break;

          default:
            Console.WriteLine("Invalid command!");
            break;
        }
      }
    }
    // Handle exceptions by printing the exception message to the console.
    catch (Exception e)
    {
      Console.WriteLine(e.Message);
    }
  }

  // Method to print the menu options to the console.
  private static void PrintMenu()
  {
    Console.WriteLine("\n?\t-\tHelp\n" +
        "cd\t-\tChange to bucket\n" +
        "ls\t-\tList Objects\n" +
        "get\t-\tGet remoteFile\n" +
        "put\t-\tPut localFile\n" +
        "q\t-\tQuit\n");
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