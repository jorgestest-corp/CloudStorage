/*
 * Cloud Storage 2022 C++ Edition - Sample Project
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
 */

#include <iostream>
#include <fstream>
#include <string>
#include "../../include/cloudstorage.h"

using namespace std;

#define LINE_LEN 80
#define MESSAGE_LEN 1024


int main()
{
    char command[LINE_LEN];     // user's command

    CloudFiles cloudfiles1;
    OAuth oauth1;

    // Welcome Message
    printf("Welcome to the Cloud Files demo.\n");
    printf("------------------------------------------------------------\n");

    printf("Which cloud service provider would you like to use to send your message? \n  [0] - Amazon S3\n  [1] - Google Storage\n  [2] - Azure Blob\n");
    fgets(command, LINE_LEN, stdin);
    if (std::stoi(command) == 0) {
        cloudfiles1.SetServiceProvider(0);
        printf("Enter your Amazon Access key: ");
        fgets(command, LINE_LEN, stdin);
        cloudfiles1.SetAccountAccessKey(command);
        printf("Enter your Amazon secret key: ");
        cloudfiles1.SetAccountSecretKey(command);
        fgets(command, LINE_LEN, stdin);
    }
    else if (std::stoi(command) == 1) {
        cloudfiles1.SetServiceProvider(9);
        oauth1.SetServerAuthURL("https://accounts.google.com/o/oauth2/auth");
        oauth1.SetServerTokenURL("https://accounts.google.com/o/oauth2/token");
        printf("Enter your OAuth Client ID: ");
        fgets(command, LINE_LEN, stdin);
        oauth1.SetClientId(command);
        printf("Enter your OAuth client secret: ");
        fgets(command, LINE_LEN, stdin);
        oauth1.SetClientSecret(command);
        printf("Authenticating...\n");
        cloudfiles1.SetAuthorization(oauth1.GetAuthorization());
        printf("Authentication successful.\n");
    }
    else if (std::stoi(command) == 2) {
        cloudfiles1.SetServiceProvider(6);
        oauth1.SetServerAuthURL("https://login.microsoftonline.com/common/oauth2/v2.0/authorize");
        oauth1.SetServerTokenURL("https://login.microsoftonline.com/common/oauth2/v2.0/token");
        printf("Enter your OAuth Client ID: ");
        fgets(command, LINE_LEN, stdin);
        oauth1.SetClientId(command);
        printf("Enter your OAuth client secret: ");
        fgets(command, LINE_LEN, stdin);
        oauth1.SetClientSecret(command);
        printf("Authenticating...\n");
        cloudfiles1.SetAuthorization(oauth1.GetAuthorization());
        printf("Authentication successful.\n");
    }
    else {
        throw std::invalid_argument("Invalid mail service provider.");
    }

    printf("What would you like to do? \n  [0] - Upload a file\n  [1] - Download a file\n  [2] - Delete a file\n  [3] - Create Directory\n  [4] - Delete Directory\n  [5] - Quit");

    fgets(command, LINE_LEN, stdin);

    while (std::stoi(command) != 5) {
        if (std::stoi(command) == 0) {
            printf("Local file path: ");
            fgets(command, LINE_LEN, stdin);
            cloudfiles1.SetLocalFile(command);
            printf("Remote file path: ");
            fgets(command, LINE_LEN, stdin);
            cloudfiles1.SetRemoteFile(command);
            cloudfiles1.Upload();
            printf("Done.\n");
        }
        else if (std::stoi(command) == 1) {
            printf("Local file path: ");
            fgets(command, LINE_LEN, stdin);
            cloudfiles1.SetLocalFile(command);
            printf("Remote file path: ");
            fgets(command, LINE_LEN, stdin);
            cloudfiles1.SetRemoteFile(command);
            cloudfiles1.Download();
            printf("Done.\n");
        }
        else if (std::stoi(command) == 2) {
            printf("File path: ");
            fgets(command, LINE_LEN, stdin);
            cloudfiles1.DeleteFile(command);
            printf("Done.\n");
        }
        else if (std::stoi(command) == 3) {
            printf("Directory name: ");
            fgets(command, LINE_LEN, stdin);
            cloudfiles1.MakeDirectory(command);
            printf("Done.\n");
        }
        else if (std::stoi(command) == 4) {
            printf("Directory name: ");
            fgets(command, LINE_LEN, stdin);
            cloudfiles1.RemoveDirectory(command);
            printf("Done.\n");
        }
        else if (std::stoi(command) == 5) {
            break;
        }
        else {
            printf("Invalid command");
        }
        printf("What would you like to do? \n  [0] - Upload a file\n  [1] - Download a file\n  [2] - Delete a file\n  [3] - Create Directory\n  [4] - Delete Directory\n  [5] - Quit");

        fgets(command, LINE_LEN, stdin);
    }
    exit(0);
}





