import json
import codecs
from pprint import pprint
import sqlite3
import collections
import re
import os
import urllib.request

SQL_DUMP_FILENAME = "tn.sql"
#JSON_FILENAME = "output.json"
SQLITE_FILENAME = "collectConnectDB.db"

FILEDIR = os.path.dirname(os.path.realpath(__file__))
PICSDIR = "".join((FILEDIR,'\pics\\'))
final_dict = collections.OrderedDict()
conn = sqlite3.connect(SQLITE_FILENAME)
db_cursor = conn.cursor()
hard_params =  ["location","year","concept","language","medium"]
hard_param_tables = ["work_locations","work_dates","work_concepts","work_languages","work_mediums"]

#print(PICSDIR)
#finds paramter in sqlite db - if non existent, adds to db and returns id
def findParameter(parameter):
    query = "SELECT parameterID FROM parameters WHERE parameter = '{}'".format(parameter.strip("'"))
    db_cursor.execute(query)
    results = db_cursor.fetchone()
    if results == None:
        query = "INSERT INTO parameters (parameter) VALUES ('{}')".format(parameter.strip("'"))
        db_cursor.execute(query)
        print(query)
        return db_cursor.lastrowid
    else:
        return results[0]

# finds attribute in sqlite db - if non existent, adds to db and returns id
def findAttribute(attribute):
    query = "SELECT attributeID FROM attributes WHERE attribute = '{}'".format(attribute.strip("'"))
    query.format(attribute.strip("\""))
    #print(query)
    db_cursor.execute(query)
    results = db_cursor.fetchone()
    if results == None:
        query = "INSERT INTO attributes (attribute) VALUES ('{}')".format(attribute.strip("'"))
        db_cursor.execute(query)
        return db_cursor.lastrowid
    else:
        return results[0]

def resolveAttributes(data,workID,parameter_idx):
    results = []
    true_results = []

    for attribute in data[hard_param_tables[parameter_idx]]:
        if attribute["work"] == workID:
            results.append(attribute[hard_params[parameter_idx]])
    if hard_params[parameter_idx] == "location":
        for loc in data["location_objects"]:
            if loc["id"] in results:
                results.remove(loc["id"])
                true_results.append(loc["name"])
    elif hard_params[parameter_idx] == "language":
        for lang in data["language_objects"]:
            if lang["id"] in results:
                results.remove(lang["id"])
                true_results.append(lang["name"])
    else:
        true_results = results

    return true_results

def resolveUser(data,uniqname):
    for user in data["users"]:
        if user["uniqname"] == uniqname:
            return user["first_name"] + " " + user["last_name"]
    return ""

def addSet(data,setID):
    for collection in data["collection_objects"]:
        if collection["id"] == setID:
            query = "INSERT INTO sets (setID,setTitle,setCreators) VALUES ( {}, '{}', '{}')".format(collection_id,collection["name"].replace("'","''").replace("[collect-connect]",""),resolveUser(data,collection["owner"]))
            #print(query)
            conn.execute(query)
            break

def addKeywords(data,workID):
    for idx,table in enumerate(hard_param_tables):

        query = "SELECT * FROM parameters WHERE parameter = '{}'".format(hard_params[idx])
        db_cursor.execute(query)
        results = db_cursor.fetchone()
        if results == None:
            query = "INSERT INTO parameters (parameterID,parameter) VALUES ( {}, '{}')".format(idx,hard_params[idx])
            db_cursor.execute(query)

        attrs = resolveAttributes(data,workID,idx)
        for attr in attrs:
            attr_id = findAttribute(attr)
            query = "INSERT INTO parameters_attributes (cardID,parameterID,attributeID) VALUES ({},{},{})".format(workID,idx,attr_id)
            db_cursor.execute(query)

    for misc in data["work_concepts"]:
        if misc["work"] == workID:
            print("Misc Work " + misc["work"] + "  Work ID: " + workID)
            param = misc["concept"]
            #print(param)
            attr = misc["id"]
            param_id = findParameter(param)
            attr_id = findAttribute(attr)
            query = "INSERT INTO parameters_attributes (cardID,parameterID,attributeID) VALUES ({},{},{})".format(workID,param_id,attr_id)
            #print(query)

def addImages(data,workID):
    work_ids = []
    for collection in data["work_images"]:
        if(collection["work"] == workID):
            #pprint(collection)
            work_ids.append(collection["work"])
    #pprint(workID)
    for work in data["work_images"]:
        if work["work"] in work_ids:
            return work["uri"]
            #query = "INSERT INTO cards (imageLocation) VALUES ( '{}' )".format(work["uri"].replace("'","''"))
            #print(query)
            #db_cursor.execute(query)
            #work_ids.remove(work["work"])
    #print(work_ids)   

def addLocation(data,workID):
    work_ids = "default"
    for collection in data["work_locations"]:
        if(collection["work"] == workID):
            work_ids = collection["location"]
            break
            #print(work_ids)
    for work in data["location_objects"]:
        #print(work["id"],work_ids)
        if (work["id"] == work_ids):
            return work["name"]

def downloadImage(URL, workTitle):
    if not URL:
	    URL = "https://s3.amazonaws.com/translationnetworkss3bucket/tn+views/symbol.png"
    #Capitalizing each word
    workTitle = "".join((x for x in workTitle.title() if not x.isspace()))
    #Removing non alphanumeric characters
    workTitle = "".join(y for y in workTitle if y.isalnum())
    #getting rid of whitespaces
    workTitle = "".join(workTitle.split())
    #adding the jpg extension to the filename
    workTitle = "".join((workTitle,".jpg"))
    #putting the first character into lower case to make the title in camelCase format
    workTitle = workTitle[0].lower() + workTitle[1:]
    #Downloading the image with and putting it in the pics dir
    #print(workTitle)
    #print(URL)
    urllib.request.urlretrieve(URL, "".join((PICSDIR,workTitle)))
    return workTitle
	

def addWorks(data,setID):
    work_ids = []
    picName = ""
    for collection in data["collection_works"]:
        if(collection["collection"] == setID):
            #pprint(collection)
            work_ids.append(collection["work"])
    #pprint(work_ids)
    for work in data["work_objects"]:
        if work["id"] in work_ids:
            imageURL = addImages(data,work["id"])
            if not imageURL:
                imageURL = "https://s3.amazonaws.com/translationnetworkss3bucket/tn+views/def_thumb.png"
            picName = downloadImage(imageURL,work["title"])
            location = addLocation(data,work["id"])
            query = "INSERT OR IGNORE INTO cards (cardID,cardDisplayTitle,cardDescription,setID,imageLocation,imageURL,Location) VALUES ( {}, '{}', '{}', {}, '{}', '{}', '{}' )".format(work["id"],work["title"].replace("'","''"),work["description"].replace("'","''"),collection_id,picName.replace("'","''"),imageURL,location)
            #print(query)
            conn.execute(query)
            addKeywords(data,work["id"])
            #addImages(data,work["id"])
            work_ids.remove(work["id"])
    #print(work_ids)

def clearDB():
	query = "DELETE FROM sets"
	db_cursor.execute(query)
	query = "DELETE FROM cards"
	db_cursor.execute(query)
	query = "DELETE FROM attributes"
	db_cursor.execute(query)
	query = "DELETE FROM parameters"
	db_cursor.execute(query)
	query = "DELETE FROM parameters_attributes"
	db_cursor.execute(query)
	

	
def processRawValues(table_name,fields,raw):
	field_index = 0
	final_list = []
	row_dict = collections.OrderedDict()
	inRow = False
	inQuote = False
	word = ""
	table_name = table_name.strip('`')
	for idx,char in enumerate(raw):
		if(char == '(' and not inQuote):
			inRow = True
		elif(char == ')' and not inQuote): #row terminator
			inRow = False
			row_dict[fields[field_index]] = word
			final_list.append(row_dict)
			row_dict = collections.OrderedDict()
			field_index = 0
			word = ""
		elif(inRow and char == ',' and not inQuote): #field terminator
			word = word.strip("'")
			word = word.replace("\\\\","\\")
			word = word.replace("\\n"," ")
			word = word.replace("\\r"," ")
			word = word.replace("\\","")
			row_dict[fields[field_index]] = word
			field_index+=1
			word = ""
		elif(char == '\''):
			word += char
			if(inQuote and raw[idx-1] != '\\'): #quote terminator
				inQuote = False
			elif(not inQuote): #quote starter
				inQuote = True
		elif inRow:
			word+=char
	#pprint(final_list)	
	final_dict[table_name] = final_list
	
	

with open(SQL_DUMP_FILENAME, "r",encoding='utf-8', errors='ignore') as data_file:
	table_name = "???"
	table_fields = []
	inTable = False
	for line in data_file:
		matchObj = re.match(r'CREATE TABLE\s+(.*?)\s',line)
		if matchObj:
			table_name = matchObj.group(1).strip()
			inTable = True
			table_fields = []
			#print(table_name)
		elif inTable:
			matchObj = re.match('\s*\`(.*?)\`',line)
			if matchObj:
				table_fields.append(matchObj.group(1))
			else:
				#pprint(table_fields)
				inTable	= False
		else:
			matchObj = re.match(r'INSERT INTO\s+(.*?)\s+VALUES\s+(.*)',line)
			if matchObj and matchObj.group(1).strip() == table_name:
				raw_values = matchObj.group(2)
				processRawValues(table_name,table_fields,raw_values)

	json_string = json.dumps(final_dict,indent=4)			

	clearDB()
	data = json.loads(json_string)
	#collection_id = input("Please enter collection ID : ")
	
	for collection in data["collection_objects"]:
		if re.match('\[collect-connect\]',collection["name"]):
			collection_id = collection["id"]
			addSet(data,collection_id)
			addWorks(data,collection_id)
	
	conn.commit()
	conn.close()