#import json
import codecs
from pprint import pprint
import sqlite3
import collections
import re
import os
import urllib.request

SQL_DUMP_FILENAME = "tn1.sql"
#JSON_FILENAME = "output.json"
SQLITE_FILENAME = "collectConnectDB.db"

FILEDIR = os.path.dirname(os.path.realpath(__file__))
PICSDIR = FILEDIR + "\pics\\"
final_dict = collections.OrderedDict()
conn = sqlite3.connect(SQLITE_FILENAME)
db_cursor = conn.cursor()

tn_tables = ["work_concepts","work_dates","work_images","work_languages","work_locations","work_mediums", \
             "work_misc","work_objects","work_people","people_objects","location_objects", \
             "language_objects","collection_objects","collection_works"]

hard_categories =  ["location","year","concept","language","medium","person"]
hard_category_tables = ["work_locations","work_dates","work_concepts","work_languages",
                     "work_mediums","work_people"]

#print(PICSDIR)

#some text is changed to low caps by TN, so it must be titled. A few tweaks are introduced here.
def myTitle(text):
    text = text.title()
    text = re.sub(r'^Ca. ', r'ca. ', text)
    text = re.sub(r' Ce$', r' CE', text)
    text = re.sub(r' Bce$', r' BCE', text)
    text = re.sub(r' Of ', r' of ', text)
    text = re.sub(r' The ', r' the ', text)
    text = re.sub(r' And ', r' and ', text)
    text = re.sub(r' For ', r' for ', text)
    text = re.sub(r' In ', r' in ', text)
    text = re.sub(r' On ', r' on ', text)
    text = re.sub(r' Out ', r' out ', text)
    text = re.sub(r' A ', r' a ', text)
    text = re.sub(r' An ', r' an ', text)
    #change possessive 'S to 's (to correct the title() function)
    text = re.sub(r"(\w\'\')(S)([^\w]|$)", r"\1s\3", text)    

    return text.strip()

#finds paramter in sqlite db - if non existent, adds to db and returns id
def findParameter(parameter):
    parameter = myTitle(parameter)
    
    query = "SELECT parameterID FROM parameters WHERE parameter = '{}'" \
            .format(parameter)
    db_cursor.execute(query)
    results = db_cursor.fetchone()
    if results == None and parameter is not None and parameter.lower() != 'null' \
       and parameter.strip() != '':
        query = "INSERT INTO parameters (parameter) VALUES ('{}')" \
                .format(parameter)
        db_cursor.execute(query)
        #print(query)
        return db_cursor.lastrowid
    elif results == None:
        return None
    else:
        return results[0]

# finds attribute in sqlite db - if non existent, adds to db and returns id
def findAttribute(attribute):
    query = "SELECT attributeID FROM attributes WHERE attribute = '{}'" \
            .format(attribute)
    db_cursor.execute(query)
    results = db_cursor.fetchone()
    if results == None and attribute is not None and attribute.lower() != 'null' \
       and attribute.strip() != '':
        query = "INSERT INTO attributes (attribute) VALUES ('{}')" \
                .format(attribute)
        db_cursor.execute(query)
        return db_cursor.lastrowid
    elif results == None:
        return None
    else:
        return results[0]

def resolveParametersAttributes(workID,cat_idx):
    results = {}
    true_results = []

    for row in final_dict[hard_category_tables[cat_idx]]:
        if row["work"] == workID:
            param = row[hard_categories[cat_idx]]
            if hard_categories[cat_idx] in ["location","person","language"]:
                results[row[hard_categories[cat_idx]]] = row["tag"]
            elif hard_categories[cat_idx] == "year":
                if int(row["approximation"]) != 0:
                    circa = "ca. "
                else:
                    circa = ""
                    
                if int(row[hard_categories[cat_idx]]) >= 0 and int(row[hard_categories[cat_idx]]) < 1500:
                    results[circa+row[hard_categories[cat_idx]]+' CE'] = row["tag"].title()
                elif int(row[hard_categories[cat_idx]]) < 0:
                    results[circa+row[hard_categories[cat_idx]][1:]+' BCE'] = row["tag"].title()
                else:
                    results[circa+row[hard_categories[cat_idx]]] = row["tag"].title()
                    
            else:
                results[row[hard_categories[cat_idx]].title()] = ''
            
    if hard_categories[cat_idx] == "location":
        for loc in final_dict["location_objects"]:
            if loc["id"] in results:
                true_results.append((loc["name"].title(),results[loc["id"]].title()))
    elif hard_categories[cat_idx] == "language":
        for lang in final_dict["language_objects"]:
            if lang["id"] in results:
                true_results.append((lang["name"].title(),results[lang["id"]].title()))
    elif hard_categories[cat_idx] == "person":
        for pers in final_dict["people_objects"]:
            if pers["id"] in results:
                full_name = ''
                if pers["first"].lower() != 'null':
                    full_name = pers["first"]
                if pers["middle"].lower() != 'null':
                    full_name = full_name+' '+pers["middle"]
                if pers["last"].lower() != 'null':
                    full_name = full_name+' '+pers["last"]
                full_name = full_name.replace("  "," ").strip()
                true_results.append((full_name.title(),results[pers["id"]].title()))
    else:
        true_results = [(x[0],x[1]) for x in results.items()]

    return true_results

##def resolveUser(uniqname):
##    for user in final_dict["users"]:
##        if user["uniqname"] == uniqname:
##            return user["first_name"] + " " + user["last_name"]
##    return ""

def addSet(setID):
    for collection in final_dict["collection_objects"]:
        if collection["id"] == setID:
            if collection["description"].lower().strip() == 'null':
                subtitle = 'NULL'
            else:
                subtitle = "'"+collection["description"].strip()+"'"
            query = "INSERT INTO sets (setID,setTitle,setSubtitle) VALUES ( {}, '{}', {})" \
                    .format(collection_id,collection["name"].replace("[collect-connect]","").strip(), \
                            subtitle)
            #print(query)
            conn.execute(query)
            break

def addKeywords(workID):
    for idx,table in enumerate(hard_category_tables):

        query = "SELECT * FROM categories WHERE category = '{}'".format(hard_categories[idx].title())
        db_cursor.execute(query)
        results = db_cursor.fetchone()
        if results == None:
            query = "INSERT INTO categories (categoryID,category) VALUES ( {}, '{}')" \
                    .format(idx,hard_categories[idx].title())
            db_cursor.execute(query)

        paramsAttrs = resolveParametersAttributes(workID,idx)
        for pa in paramsAttrs:
            # this is to avoid a last-minute issue to display card info correctly in CollectConnect
            # Located information is now on the cards as "Location"
            if pa[1] != "Located":
                paramID = findParameter(pa[0])
                if paramID is not None:
                    attrID = findAttribute(pa[1])
                    if attrID is None:
                        attrID = 'NULL'
                    query = "INSERT INTO categories_parameters_attributes (cardID,categoryID,parameterID,attributeID) VALUES ({},{},{},{})" \
                            .format(workID,idx,paramID,attrID)
                    db_cursor.execute(query)

##    for misc in final_dict["work_concepts"]:
##        if misc["work"] == workID:
##            #print("Misc Work " + misc["work"] + "  Work ID: " + workID)
##            param = misc["concept"]
##            #print(param)
##            attr = misc["id"]
##            param_id = findParameter(param)
##            attr_id = findAttribute(attr)
##            query = "INSERT INTO parameters_attributes (cardID,parameterID,attributeID) VALUES ({},{},{})" \
##                    .format(workID,param_id,attr_id)
##            #print(query)

def addImages(workID):
    for misc in final_dict["work_misc"]:
        if misc["work"] == workID and misc["param"].lower() == "imagefilename":
            #print(misc["value"])
            #upper() here because TN does mysterious recapitalization and it confused me, so we're just going
            #with the user-side display of filenames
            return 'http://www-personal.umich.edu/~posch/collect-connect/'+misc["value"][0].upper()+misc["value"][1:]
        
    for collection in final_dict["work_images"]:
        if(collection["work"] == workID):
            #pprint(collection)
            return collection["uri"]
    #pprint(workID)

def addLibrary(workID):
    libraryID = ''
    
    for collection in final_dict["work_locations"]:
        #using "Located" as arbitrary tag for which library the work is held
        if(collection["work"] == workID and collection["tag"].lower() == "located"):
            libraryID = collection["location"]
            break
    if libraryID != '':    
        for location in final_dict["location_objects"]:
            if (location["id"] == libraryID):    
                return myTitle(location["name"]) #because TN changes name to lowercase
    else:
        return None
    
def downloadImage(URL, workTitle):
    if not "www-personal.umich.edu" in URL:    
        #Capitalizing each word
        fileName = "".join((x for x in workTitle.title() if not x.isspace()))
        #Removing non alphanumeric characters
        fileName = "".join(y for y in fileName if y.isalnum())
        #getting rid of whitespaces
        fileName = "".join(fileName.split())
        #adding the image file type extension to the filename
        fileName = "".join((fileName,URL[URL.rfind("."):]))
        #putting the first character into lower case to make the title in camelCase format
        fileName = '_WARNING-low-res-image_'+fileName[0].lower() + fileName[1:]
        #Downloading the image with and putting it in the pics dir
        print("WARNING: low-res image used for "+ workTitle)
        #print(URL)
    else:
        fileName = URL[URL.rfind("/")+1:]

    try:
        urllib.request.urlretrieve(URL, "".join((PICSDIR,fileName)))
    except:
        fileName = '_ERROR-image-not-found_'+fileName
        print("ERROR: image not found for "+workTitle+" @ "+URL)
        URL = "https://s3.amazonaws.com/translationnetworkss3bucket/tn+views/symbol.png"
        urllib.request.urlretrieve(URL, "".join((PICSDIR,fileName)))
    return fileName
	

def addWorks(setID):
    work_ids = []
    picName = ""
    for collection in final_dict["collection_works"]:
        if(collection["collection"] == setID):
            #pprint(collection)
            work_ids.append(collection["work"])
    #pprint(work_ids)
    for work in final_dict["work_objects"]:
        #if statement below checks if the object is in the ids, is not null,
        #and has a descrip length of atleast 5 characters (which prevents the string form of "NULL"
        if work["id"] in work_ids and work["description"] is not None \
           and len(work["description"]) > 4:
            #print( work["description"].replace("'","''")+"   Length   "+str(len(work["description"])))
            imageURL = addImages(work["id"])
            if not imageURL:
                imageURL = "https://s3.amazonaws.com/translationnetworkss3bucket/tn+views/symbol.png"
            imageFileName = downloadImage(imageURL,work["title"])
            #NOTE: we need another table if one card belongs to more than one set
            library = addLibrary(work["id"])
            if library is None:
                library = 'NULL'
                print("ERROR: no cards.Location value for: "+work["title"])
            else:
                if library == "Art Architecture Engineering Library":
                    library = "AAEL"
                elif library == "Kelsey Museum of Archaeology":
                    library = "Kelsey"
                elif library == "Language Resource Center":
                    library = "LRC"
                elif library == "Mardigian Library":
                    library = "Mardigian"
                elif library == "Bentley Historical Library":
                    library = "Bentley"
                elif library == "Nichols Arboretum":
                    library = "The Arb"
                elif library == "Clements Library":
                    library = "Clements"
                elif library == "Clark Library":
                    library = "Clark"
                elif library == "University of Michigan Museum of Art":
                    library = "UMMA"
                elif library == "University of Michigan Museum of Anthropological Archaeology":
                    library = "UMMAA"
                else:
                    library = "Hatcher"
                
                library = "'"+library+"'"
            if work["subtitle"].lower() != 'null':
                title2 = "'"+work["subtitle"]+"'" #no title() here. unnecessary since TN does not modify original.
            else:
                title2 = 'NULL'

            query = "INSERT OR IGNORE INTO cards (cardID,cardDisplayTitle,cardFullTitle,cardDescription,setID,imageFileName,imageURL,Location) VALUES ({},'{}',{},'{}',{},'{}','{}',{})" \
                    .format(work["id"],work["title"],title2,work["description"],collection_id, \
                            imageFileName,imageURL,library)
            conn.execute(query)
            addKeywords(work["id"])
            #work_ids.remove(work["id"])

def clearDB():
    #delete pics folder
    for file in os.listdir(PICSDIR):
        os.remove(PICSDIR+file)
        
    query = "DELETE FROM sets"
    db_cursor.execute(query)
    query = "DELETE FROM cards"
    db_cursor.execute(query)
    query = "DELETE FROM attributes"
    db_cursor.execute(query)
    query = "DELETE FROM parameters"
    db_cursor.execute(query)
    query = "DELETE FROM categories"
    db_cursor.execute(query)
    query = "DELETE FROM categories_parameters_attributes"
    db_cursor.execute(query)

def processRawValues(table_name,fields,raw):
    field_index = 0
    final_list = []
    row_dict = collections.OrderedDict()
    inRow = False
    inQuote = False
    word = ""
    for idx,char in enumerate(raw):
        if(char == '(' and not inQuote):
            inRow = True
        elif(char == ')' and not inQuote): #row terminator
            inRow = False
            word = word.strip("'").strip()
            word = word.replace("\\'","''")
            word = word.replace('\\"','"')
            word = word.replace("\\\\","\\")
            word = word.replace("\t"," ")
            #word = word.replace("\n"," ")
            #word = word.replace("\r"," ")
            row_dict[fields[field_index]] = word
            final_list.append(row_dict)
            row_dict = collections.OrderedDict()
            field_index = 0
            word = ""
        elif(inRow and char == ',' and not inQuote): #field terminator
            word = word.strip("'").strip()
            word = word.replace("\\'","''")
            word = word.replace('\\"','"')
            word = word.replace("\\\\","\\")
            word = word.replace("\t"," ")
            #word = word.replace("\n"," ")
            #word = word.replace("\r"," ")
            row_dict[fields[field_index]] = word
            field_index+=1
            word = ""
        elif(char == "'"):
            word += char
            #quote terminator
            if(inQuote and raw[idx-1] != '\\' or \
               inQuote and raw[idx-1] == '\\' and raw[idx-2] == '\\'): #if strange, final backslash
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
            table_name = matchObj.group(1).strip().strip('`').strip().lower()

            if table_name in tn_tables:      
                inTable = True
                table_fields = []
                #print(table_name)
            else:
                table_name = "???"
                
        elif inTable:
            matchObj = re.match(r'\s*\`(.*?)\`',line)
            if matchObj:
                 table_fields.append(matchObj.group(1).strip().strip('`').strip().lower())
            else:
                #pprint(table_fields)
                inTable	= False
        elif table_name != "???":
            matchObj = re.match(r'INSERT INTO\s+(.*?)\s+VALUES\s+(.*)',line)
            if matchObj and matchObj.group(1).strip().strip('`').strip().lower() == table_name:
                raw_values = matchObj.group(2).strip().strip(';')
                #print(raw_values)
                processRawValues(table_name,table_fields,raw_values)
                table_name = "???"

    #json_string = json.dumps(final_dict,indent=4)			

    clearDB()
    #data = json.loads(json_string)
    #collection_id = input("Please enter collection ID : ")
    #import pdb; pdb.set_trace()

    for collection in final_dict["collection_objects"]:
        #print(collection["id"], int(collection["id"]) in working_collections, int(collection["deleted"])==0)
        if re.match(r'\[collect-connect\]',collection["name"]) and int(collection["deleted"]) == 0:
            collection_id = collection["id"]
            print("")
            print("...processing set: "+collection["name"]+"...")
            addSet(collection_id)
            addWorks(collection_id)
    
    conn.commit()
    conn.close()
            
