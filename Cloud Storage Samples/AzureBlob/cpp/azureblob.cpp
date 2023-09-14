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



class MyBlob : public AzureBlob
{
public:

	virtual int FireContainerList(AzureBlobContainerListEventParams *e)
	{
		printf("%s\n", e->Name);
		return 0;
	}

	virtual int FireBlobList(AzureBlobBlobListEventParams *e)
	{
		printf("%s\n", e->Name);
		return 0;
	}

	virtual int FireError(AzureBlobErrorEventParams *e)
	{
		printf("Error Code: %d, Description: %s", e->ErrorCode, e->Description);
		return 0;
	}
};

void printoptions()
{
	printf( "Type ? to select this menu, a number 1-5 to select the following choices:\n"
	        "1.  List Blobs\n"
	        "2.  Get Blob\n"
	        "3.  Put Blob\n"
	        "4.  Quit\n");
}


int main(int argc, char **argv)
{
	MyBlob blob;
	char buffer[LINE_LEN+1];
	int ret_code = 0;
	int size = 0;

	printf("Azure Account: ");
	scanf("%80s",buffer);
	blob.SetAccount(buffer);

	printf("Access Key: ");
	scanf("%100s",buffer);
	blob.SetAccessKey(buffer);

	printf("Listing Available Containers ...\n\n");
	if( ret_code = blob.ListContainers() ) goto error;

	printf("\nBlob Container to Examine: ");
	scanf("%80s",buffer);
	blob.SetContainer(buffer);

	printoptions();
	while (1)
	{
		printf( "blob> " );
		scanf("%80s",buffer);

		if ( ! strcmp(buffer, "?") )
		{
			printoptions();
		}

		else if ( ! strcmp(buffer, "1") )
		{
			if( ret_code = blob.ListBlobs() ) goto error;
		}

		else if ( ! strcmp(buffer, "2") )
		{
			printf("Which Blob?: ");
			scanf("%80s",buffer);
			if ( ret_code = blob.GetBlob(buffer) ) goto error;
			char * data;
			if ( ret_code = blob.GetBlobData(data, size) ) goto error;
			printf("Contents of blob %s :\n",buffer);
			for(int i=0; i < size; i++)
			{
				printf("%c",data[i]);
			}
			printf("\n");
		}

		else if ( ! strcmp(buffer, "3") )
		{
			char blobname[81];
			printf("Name of new blob: ");
			scanf("%80s",blobname);
			printf("Local file to upload: ");
			scanf("%80s",buffer);
			blob.SetLocalFile(buffer);
			if( ret_code = blob.CreateBlob(blobname, 0, -1) ) goto error;
			printf("Blob %s created.\n",blobname);
			blob.SetLocalFile("");
		}

		else if ( ! strcmp(buffer, "4") )
		{
			exit(0);
		}

		else if ( ! strcmp(buffer, "") )
		{
			// Do nothing
		}
		else
		{
			printf( "Please select a number from [1-4].\n" );
		} // end of command checking

error:
		if(ret_code)
		{
			printf("\nError: %d", ret_code);
			if (blob.GetLastError())
			{
				printf( " \"%s\"\n", blob.GetLastError() );
			}
		}

	}  // end of main while loop
	return ret_code;
}



