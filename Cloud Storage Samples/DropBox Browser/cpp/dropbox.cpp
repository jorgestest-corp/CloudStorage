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

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include "../../include/cloudstorage.h"
#define LINE_LEN 100

class MyDropBox : public Dropbox
{
public:
	
	virtual int FireSSLServerAuthentication(DropboxSSLServerAuthenticationEventParams *e)
	{
		e->Accept = true;
		return 0;
	}
};

MyDropBox dropbox1;

void printOptions()
{
	printf( "Type ? to select this menu, a number 1-5 to select the following choices:\n"
	        "1.  List Documents\n"
	        "2.  Download Document\n"
	        "3.  Upload Document\n"
	        "4.  Delete Document\n"
	        "5.  Quit\n");
}

char* resolveResourceType(int type) {
  switch (type)
  {
    case BRT_FILE: return "File";
    case BRT_FOLDER: return "Folder";
    default: return "Unknown";
  }
}

void displayDocuments(){

	int ret_code = dropbox1.ListResources("");
	if(ret_code)
		goto error;

	printf("%-3.1s%-40.38s%-10.8s%-25.23s\n\n","#","Title","Type","Modified Date");

	for(int i=0;i<dropbox1.GetResourceCount();i++)
	{
		printf("%-3.1d%-40.38s%-10.8s%-25.23s\n",i,dropbox1.GetResourceName(i),resolveResourceType(dropbox1.GetResourceType(i)),dropbox1.GetResourceModifiedTime(i));
	}
	return;

error:
	printf("\nError: %d", ret_code);
	if (dropbox1.GetLastError())
	{
		printf( " \"%s\"\n", dropbox1.GetLastError() );
	}
	printf("Press any key to continue");
	fflush(stdin);
	getchar();
	exit(ret_code);
}

int main(int argc, char **argv)
{	
  char buffer[LINE_LEN+1];
  int ret_code = 0;

  printf("***************************************************************\n");
  printf("* This demo shows how to use the DropBox component to list,    *\n");
  printf("* download, upload, and delete documents. To begin, you will   *\n");
  printf("* first need to setup an app in the dropbox developer console  *\n");
  printf("* to obtain your Client Id and Client Secret. You will also    *\n");
  printf("* need to setup a Redirect URI to http://localhost:PORT, where *\n");
  printf("* PORT is the port number configured in the code.              *\n");
  printf("***************************************************************\n\n");

  printf("Client Id: ");
  fgets(buffer, LINE_LEN, stdin);
  buffer[strlen(buffer)-1] = '\0';
  dropbox1.SetOAuthClientId(buffer); 
    
  printf("Client Secret: ");
  fgets(buffer, LINE_LEN, stdin);
  buffer[strlen(buffer)-1] = '\0';
  dropbox1.SetOAuthClientSecret(buffer);
  
  dropbox1.SetOAuthServerAuthURL("https://www.dropbox.com/oauth2/authorize");
  dropbox1.SetOAuthServerTokenURL("https://api.dropboxapi.com/oauth2/token");
  dropbox1.Config("OAuthWebServerPort=7777");

  printf("\nPerforming OAuth authentication ...\n");

  dropbox1.Authorize();

  ret_code = dropbox1.GetLastErrorCode();

  if(ret_code)
    goto error;

	printf("\n");
	printOptions();

	while (1)
	{
		printf( "\n> " );
		scanf("%80s",buffer);

		if ( ! strcmp(buffer, "?") )
		{
			printOptions();
		}

		else if ( ! strcmp(buffer, "1") ) //List documents
		{
			displayDocuments();
		}

		else if ( ! strcmp(buffer, "2") ) //Download document
		{
			printf("\nDocument Number: ");
			scanf("%80s",buffer);
			int docNum = atoi(buffer);

			printf("Local Download Directory: ");
			scanf("%80s",buffer);
			strcat(buffer,"\\");
			strcat(buffer,dropbox1.GetResourceName(docNum));
			dropbox1.SetLocalFile(buffer);

			ret_code = dropbox1.DownloadFile(dropbox1.GetResourceId(docNum)); //Use the default file format
			if(ret_code)
				goto error;

			printf("\n Download Successful\n");
		}

		else if ( ! strcmp(buffer, "3") ) //Upload document
		{
			printf("\nLocal File: ");
			scanf("%80s",buffer);

			dropbox1.SetLocalFile(buffer);
			
			printf("Name of New File on Server: ");
			scanf("%80s",buffer);
      dropbox1.UploadFile(buffer);
			ret_code = dropbox1.GetLastErrorCode();

			if(ret_code)
				goto error;

			displayDocuments();
		}

		else if ( ! strcmp(buffer, "4") ) //Delete Document
		{
			printf("\nDocument Number: ");
			scanf("%80s",buffer);
			char* fileToDelete = dropbox1.GetResourceId(atoi(buffer));

			ret_code = dropbox1.DeleteResource(fileToDelete);

			if(ret_code)
				goto error;

			displayDocuments();
		}

		else if ( ! strcmp(buffer, "5") )
		{
			exit(0);
		}

		else if ( ! strcmp(buffer, "") )
		{
			// Do nothing
		}
		else
		{
			printf( "Please select a number from [1-5].\n" );
		} // end of command checking
	}  // end of main while loop
	return ret_code;

error:
	printf("\nError: %d", ret_code);
	if (dropbox1.GetLastErrorCode())
	{
		printf( " \"%s\"\n", dropbox1.GetLastError() );
	}
	printf("Press any key to continue");
	fflush(stdin);
	getchar();
	return ret_code;

}

