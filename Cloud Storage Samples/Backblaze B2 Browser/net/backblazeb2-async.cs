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
using System.Linq;
public class Backblazeb2Demo
{
  // Declare private static fields for BackblazeB2 object and currentBucket string.
  private static Backblazeb2 backblazeb2;

  // Define Main function as an asynchronous task.
  static async Task Main(string[] args)
  {
    try
    {
      //Prompt the user to enter application key ID and the corresponding application key.
      Console.Write("Backblaze B2 Application Key Id: ");
      var keyID = Console.ReadLine();

      Console.Write("Backblaze B2 Application Key: ");
      var key = Console.ReadLine();

      // Initialize the BackblazeB2 object and set application key and its ID .
      backblazeb2 = new Backblazeb2();
      backblazeb2.ApplicationKeyId = keyID;
      backblazeb2.ApplicationKey = key;


      // Print bucket list title and then list all the buckets.
      Console.WriteLine("\nBucket List:\n");
      await backblazeb2.ListBuckets();
      foreach (var bucket in backblazeb2.Buckets)
      {
        Console.WriteLine($"Name: {bucket.Name}\t ID: {bucket.Id}");
      }

      // Print the menu options to the console.
      PrintMenu();

      // Loop until user enters "q" for quit.
      string buffer = "";
      string bucketID = "";
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
            bucketID = Console.ReadLine();
            Console.WriteLine($"Current bucket ID is now {bucketID}.");
            break;

          case "ls":
            // List all files in the current bucket.
            try
            {
              await backblazeb2.ListFiles(bucketID);
            }
            catch (CloudStorageBackblazeb2Exception ex)
            {
              Console.WriteLine($"{ex.Message}");
              break;
            }
            foreach (var entry in backblazeb2.Files)
            {
              Console.WriteLine(entry.Name);
            }
            break;

          case "get":
            // Get a file from the current bucket.
            Console.Write("Which file: ");
            var fileName = Console.ReadLine();
            await backblazeb2.GetBucketInfo(bucketID);
            var bucketInfo = backblazeb2.Buckets.FirstOrDefault(x => x.Id == bucketID);
            await backblazeb2.DownloadFile(bucketInfo.Name, fileName);
            Console.WriteLine("The content of the file:\n=========================\n" + backblazeb2.FileData + "\n=========================\n");
            break;

          case "put":
            // Upload a local file to the current bucket as a new file.
            Console.Write("Name of new object: ");
            var newObject = Console.ReadLine();
            Console.Write("Local file to upload: ");
            backblazeb2.LocalFile = Console.ReadLine();
            await backblazeb2.UploadFile(bucketID, newObject);
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