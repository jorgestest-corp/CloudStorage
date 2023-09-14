# 
# Cloud Storage 2022 Python Edition - Sample Project
# 
# This sample project demonstrates the usage of Cloud Storage in a 
# simple, straightforward way. It is not intended to be a complete 
# application. Error handling and other checks are simplified for clarity.
# 
# www.nsoftware.com/cloudstorage
# 
# This code is subject to the terms and conditions specified in the 
# corresponding product license agreement which outlines the authorized 
# usage and restrictions.
# 

import sys
import string
from cloudstorage import *

input = sys.hexversion<0x03000000 and raw_input or input



#Global Variables#
currentFolderId = ""
currentParentId = ""
subdomain = ""
authStr = ""
sharefile = ShareFile()

#  NOTE: ShareFile requires a registered redirect URI which is https://
#        and explicitly disallows localhost. A valid public domain with SSL enabled must
#        be specific when creating your ClientId and ClientSecret values
#        
#        Visit https://api.sharefile.com/apikeys to create your credentials
#        
#  IMPORTANT: 
#       Because of the ShareFile restrictions detailed above this demo
#       make use of an existing public URL (oauth.nsoftware.com)
#       This should NOT be used in production. It is only used here to
#       facilitate running of this demo. In a real application you should
#       register your own Redirect URI.
#       
#       For any questions please contact support@nsoftware.com 
def authenticate():
  global authStr, subdomain

  oauth = OAuth()
  oauth.set_client_profile(0) #Application
  oauth.set_client_id(prompt("Client ID", "Hb7kR4d6mBvQxnOe9Eg6V8CMIVjQ67vD"))
  oauth.set_client_secret(prompt("Client Secret", "jHAZH6f1Ac5O9krW9az3fLYFEZW4KoF1sr1hqDoVgrvv1zwc"))
  oauth.set_server_auth_url("https://secure.sharefile.com/oauth/authorize")
  oauth.set_server_token_url("https://" + subdomain + ".sharefile.com/oauth/token")

  oauth.set_web_server_port(7878) #Specify a particular port, as it will be used in the redirect.
  oauth.start_web_server() #In most cases, the webserver is automatically started when get_authorization is called.  In this case, due to the use of oauth.nsoftware.com, it is started manually.
  oauth.set_return_url("https://oauth.nsoftware.com/oauthdemo") #This URL should NOT be used in a production application.  It is only for testing purposes.
  oauth.add_param("state", "http://localhost:" + str(oauth.get_web_server_port())) #The oauth.nsoftware.com server uses this value to redirect to the local server embedded within the OAuth class.
  
  #oauth.set_refresh_token("") #Set to value from previous runs to expedite the login process.
  authStr = oauth.get_authorization()
  #print(oauth.get_refresh_token()) #refresh_token may be used to automatically obtain a new token without user interaction.

def printMenu():
  print(
  """
  Type ? to view this menu, or type one of the commands below to perform the corresponding operation:
	ls    - List Items
	cd    - Change Folders
	get   - Download Items
	put   - Upload Items
	del   - Delete Items
	mkdir - Create Folder
	req   - Create Request Link
	shr   - Create Share Link
	q     - Quit
  """)

def displayList(folderId):
  global authStr, subdomain, currentFolderId, currentParentId, sharefile
  sharefile.set_account_subdomain(subdomain)
  sharefile.set_authorization(authStr)

  sharefile.get_item_info(folderId)
  currentFolderId = sharefile.get_item_id(0)
  currentParentId = sharefile.get_item_parent_id(0)
  frmStr = "%-3.2s%-40.38s%-10.8s%-25.23s"
  print(frmStr % ("#","Title","Type","Creation Date"))
  print(frmStr % ("-1","..","Folder","")) #Parent Directory

  sharefile.list_items(currentFolderId)
  for i in range(sharefile.item_count):
    print( frmStr % (str(i), sharefile.get_item_name(i), resolveItemType(sharefile.get_item_type(i)) 
      ,sharefile.get_item_creation_date(i)))
  return

def resolveItemType(type):
  if type == 0:
    return "File"
  elif type == 1:
    return "Folder"
  elif type == 2:
    return "Link"
  elif type == 3:
    return "Note"
  elif type == 4:
    return "Symbolic Link"
  else:
    return "Unknown"


def prompt(prompt, default):
  res = ""
  res = input(prompt + " [" + default + "]: ")
  if res == "":
    res = default
  return res
  
def main():
  global subdomain, currentFolderId, currentParentId, sharefile
  print("*****************************************************************")
  print("* This demo shows how to use the ShareFile component to list,   *")
  print("* upload, download, delete, and share documents from ShareFile. *")
  print("* Default prompt values are in [ ] where applicable.            *")
  print("*****************************************************************")
  subdomain = input("Account Subdomain: ")

  authenticate()

  displayList("home")
  printMenu()
  response = ""
  while not response.startswith('q') :
    response = input(">")
    if response == "ls":
      displayList(currentFolderId)
    elif response == "cd":
      cd_res = prompt("Number corresponding to folder you wish to view", str(-1))
      if int(cd_res) < 0:
        displayList(currentParentId)
      elif sharefile.get_item_type(int(cd_res)) == 1 :
        displayList(sharefile.get_item_id(int(cd_res)))
      else:
        print("Please choose a valid number corresponding to a folder.")
    elif response == "get":
      get_num_res = input("Number corresponding to item you wish to download: ")
      get_dir_res = prompt("Directory in which to download", "./")
      if not os.path.isdir(get_dir_res):
        print("Please choose a valid directory.")
        break
      filename = get_dir_res + sharefile.get_item_name(int(get_num_res))
      if sharefile.get_item_type(int(get_num_res)) == 1:
        filename += ".zip"
      sharefile.set_local_file(filename)
      sharefile.download_file(sharefile.get_item_id(int(get_num_res)))
      print("File downloaded to: " + filename)
    elif response == "put":
      put_file_res = input("Path to file that will be uploaded: ")
      if not os.path.isfile(put_file_res):
        print("Please input valid file.")
        break
      sharefile.set_local_file(put_file_res)
      sharefile.upload_file(os.path.basename(put_file_res), currentFolderId)
      print("File uploaded with name " + os.path.basename(put_file_res))
      displayList(currentFolderId)
    elif response == "del":
      del_num_res = input("Number corresponding to item you wish to delete: ")
      sharefile.delete_item(sharefile.get_item_id(int(del_num_res)))
      displayList(currentFolderId)
    elif response == "mkdir":
      mkdir_name_res = input("Name of new folder: ")
      sharefile.create_folder(mkdir_name_res, currentFolderId)
      displayList(currentFolderId)
    elif response == "req":
      print(sharefile.create_request_link(currentFolderId))
    elif response == "shr":
      shr_num_res = input("Number corresponding to item you wish to share: ")
      print(sharefile.create_link(sharefile.get_item_id(int(shr_num_res))))
    elif response == "q":
      exit()
    else:
      printMenu()

main()

