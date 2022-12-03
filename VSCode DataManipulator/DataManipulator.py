import datetime
import os
import sys

shotHistoryFile = "C:/Users/popta/Documents/GitHub/AI Tanks/Assets/Scripts/AI/ShotHistory/Main/ShotHistory.txt"
rootDir = "C:/Users/popta/Documents/GitHub/AI Tanks/Assets/Scripts/AI/ShotHistory/Main"
player1Dir = "C:/Users/popta/Documents/GitHub/AI Tanks/Assets/Scripts/AI/ShotHistory/Player1/"
player2Dir = "C:/Users/popta/Documents/GitHub/AI Tanks/Assets/Scripts/AI/ShotHistory/Player2/"

def main():
    
    starttime = datetime.datetime.now().replace(microsecond=0)
    program_startTime = starttime

    #--------------------------------------------------

    print("\nStarting to read: ", shotHistoryFile)
    print("Start time: ",starttime)

    player1list, player2list = preProcessData()
    
    endtime = datetime.datetime.now().replace(microsecond=0)
    print("End time: ",endtime)
    print("Total time: ", endtime-starttime)
    print("Finished reading: ", shotHistoryFile, ".")

    #-------------------------------------------------
    
    starttime = datetime.datetime.now().replace(microsecond=0)
    print("\nStarting to save Player 1 data...")
    print("Start time: ",starttime)

    save(player1list, player1Dir)

    endtime = datetime.datetime.now().replace(microsecond=0)
    print("End time: ",endtime)
    print("Total time: ", endtime-starttime)
    print("Finished saving Player 1 data.")

    #-------------------------------------------------
    
    starttime = datetime.datetime.now().replace(microsecond=0)
    print("\nStarting to save Player 2 data...")
    print("Start time: ",starttime)

    save(player2list, player2Dir)

    endtime = datetime.datetime.now().replace(microsecond=0)
    print("End time: ",endtime)
    print("Total time: ", endtime-starttime)
    print("Finished saving Player 2 data.")

    #--------------------------------------------------
    
    program_endTime = endtime
    print("\nTotal program execution time: ", program_endTime-program_startTime)

def save(playerlist: list, playerDir):
    if(not os.path.exists(playerDir)):
        os.mkdir(playerDir)
        
    wind = -10
    for i in range(0, 21):
        filename = playerDir + str(wind) + ".txt"
        file = open(filename, "w")
        entry_count = 0
        startTime = datetime.datetime.now().replace(microsecond=0)
        for j in range(0, len(playerlist)):
            if(playerlist[0][2] == wind):
                file.write(str(playerlist[0])[1:-1]+"\n") #substring [1:-1] removes the (...) surrounding the tuple
                playerlist.pop(0)
                entry_count += 1
            else: break
        file.close()
        endTime = datetime.datetime.now().replace(microsecond=0)

        print("Saved! -> ", filename, "->", entry_count, "entries. -> Time to complete: " + str(endTime - startTime))
        wind += 1

def preProcessData():
    def util(string: str):        
        try:
            return string[0:string.index(".")+3]
        except:
            return string
    
    file = open(shotHistoryFile, 'r')
    lines = file.readlines()
    file.close()
    print("\nProcessing", len(lines), "data entries...")
    player1set = set()
    player2set = set()
    lines.remove(lines[0])    
    lines.remove(lines[0])
    for line in lines:
        strippedLine0 = line.split("|")[0]
        strippedLine1 = line.split("|")[1]
        strippedLine2 = line.split("|")[2]
        strippedLine3 = line.split("|")[3]
        strippedLine4 = line.split("|")[4]
        
        if(line.split("|")[1].strip().startswith("-")):
            player1set.add(((float)(util(strippedLine0)), 
                                    (float)(util(strippedLine1)), 
                                        (int)(strippedLine2), 
                                            (float)(util(strippedLine3)), 
                                                (float)(util(strippedLine4))))
        else:
            player2set.add(((float)(util(strippedLine0)), 
                                    (float)(util(strippedLine1)), 
                                        (int)(strippedLine2), 
                                            (float)(util(strippedLine3)), 
                                                (float)(util(strippedLine4))))
    
    player1list = list(player1set)
    player2list = list(player2set)

    player1list.sort(key = lambda x: x[2])
    player2list.sort(key = lambda x: x[2])

    print("Processing completed!", len(lines) - (len(player1list)+len(player2list)), "duplicates removed.\n")

    return player1list, player2list

def truncate():
    starttime = datetime.datetime.now().replace(microsecond=0)
    print("\nStarting to truncate data...")
    print("Start time: ",starttime)
   
    file = open(shotHistoryFile, 'r')
    lines = file.readlines()
    file.truncate
    file.close()
    lines.remove(lines[0])    
    lines.remove(lines[0])
    file = open(shotHistoryFile+"_1", 'w')
    for i in range(0, len(lines)):
        size = os.path.getsize(shotHistoryFile+"_1")
        if(size < 100000000):
            file.write(lines[len(lines) - 1 - i])
        else: break
    file.close()
    
    endtime = datetime.datetime.now().replace(microsecond=0)
    print("End time: ",endtime)
    print("Total time: ", endtime-starttime)


if __name__ == "__main__":
    truncate()
