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



class MyWasabi : public Wasabi
{
public:

	virtual int FireObjectList(WasabiObjectListEventParams *e)
	{
		printf("%s\n", e->ObjectName);
		return 0;
	}

	virtual int FireBucketList(WasabiBucketListEventParams *e)
	{
		printf("%s\n", e->BucketName);
		return 0;
	}

	virtual int FireError(WasabiErrorEventParams *e)
	{
		printf("Error Code: %d, Description: %s",e->ErrorCode,e->Description);
		return 0;
	}

};

void printoptions()
{
	printf( "Type ? to select this menu, a number 1-5 to select the following choices:\n"
	        "1.  List Objects\n"
	        "2.  Get Object\n"
	        "3.  Get Object Link\n"
	        "4.  Put Object\n"
	        "5.  Quit\n");
}


int main(int argc, char **argv)
{

	MyWasabi wasabi;
	char buffer[LINE_LEN+1];
	int ret_code = 0;
	int size = 0;

	printf("Wasabi Access Key: ");
	scanf("%80s",buffer);
	wasabi.SetAccessKey(buffer);
	printf("Wasabi Secret Key: ");
	scanf("%80s",buffer);
	wasabi.SetSecretKey(buffer);

	printf("Available buckets:\n");
	if( ret_code = wasabi.ListBuckets() ) goto error;

	printf("\nWasabi Bucket to Examine: ");
	scanf("%80s",buffer);
	wasabi.SetBucket(buffer);

	printoptions();
	while (1)
	{
		printf( "wasabi> " );
		scanf("%80s",buffer);

		if ( ! strcmp(buffer, "?") )
		{
			printoptions();
		}

		else if ( ! strcmp(buffer, "1") )
		{
			if( ret_code = wasabi.ListObjects() ) goto error;
		}

		else if ( ! strcmp(buffer, "2") )
		{
			printf("Which object?: ");
			scanf("%80s",buffer);
			if ( ret_code = wasabi.GetObject(buffer) ) goto error;
			char * data;
			if ( ret_code = wasabi.GetObjectData(data, size) ) goto error;
			printf("Contents of object %s :\n",buffer);
			for(int i=0; i < size; i++)
			{
				printf("%c",data[i]);
			}
			printf("\n");
		}

		else if ( ! strcmp(buffer, "3") )
		{
			printf("Which object?: ");
			scanf("%80s",buffer);
			printf("%s\n", wasabi.GetLink(buffer, 60));
		}

		else if ( ! strcmp(buffer, "4") )
		{
			char objname[81];
			printf("Name of new object: ");
			scanf("%80s",objname);
			printf("Local file to upload: ");
			scanf("%80s",buffer);
			wasabi.SetLocalFile(buffer);
			if( ret_code = wasabi.CreateObject(objname) ) goto error;
			printf("Object %s created.\n",objname);
			wasabi.SetLocalFile("");
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
	if (wasabi.GetLastError())
	{
		printf( " \"%s\"\n", wasabi.GetLastError() );
	}
	return ret_code;

}


