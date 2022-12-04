import datetime
import os
import sys

shotHistoryFile = "C:/Users/popta/Documents/GitHub/AI Tanks/Assets/Scripts/AI/ShotHistory/Main/ShotHistory_1.txt"
rootDir = "C:/Users/popta/Documents/GitHub/AI Tanks/Assets/Scripts/AI/ShotHistory/Main/"
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
    bin = dict()
    bin
    wind = -10
    y_dis = playerlist[0][1]
    for i in range(0, 21):
        filename = playerDir + str(wind) + ".txt"
        file = open(filename, "w")
        entry_count = 0
        startTime = datetime.datetime.now().replace(microsecond=0)
        if(not len(playerlist) == 0 and playerlist[0][2] == wind):
            while(not len(playerlist) == 0 and playerlist[0][2] == wind):
                x_dis = str(playerlist[0][0])[0:str(playerlist[0][0]).find('.')]
                angle = 0
                power = 0
                avg_counter = 0
                #if(bin.get(x_dis)):
                #   x_dis_list = bin[x_dis]
                #   x_dis_list[3]+= playerlist[0][3]#angle 
                #   x_dis_list[4]+= playerlist[0][4]#power
                #   x_dis_list[5]+= 1#counter
                #else:
                #    x_dis_list = [x_dis, playerlist[0][1], playerlist[0][2], playerlist[0][3],playerlist[0][4],0]
                #    bin[x_dis] = x_dis_list    
                                    
                while(not len(playerlist) == 0 and str(playerlist[0][0]).startswith(x_dis)):
                    angle += playerlist[0][3]
                    power += playerlist[0][4]
                    playerlist.pop(0)
                    entry_count += 1
                    avg_counter += 1
                avg_angle = str(angle/avg_counter)
                avg_power = str(power/avg_counter)
                file.write(str(x_dis + "," + str(y_dis) + "," + str(wind) + "," + avg_angle[0:avg_angle.index(".")+4] + "," + avg_power[0:avg_power.index(".")+4] + "\n")) #substring [1:-1] removes the (...) surrounding the tuple
        else: break
        file.close()
        endTime = datetime.datetime.now().replace(microsecond=0)

        print("Saved! -> ", filename, "->", entry_count, "entries. -> Time to complete: " + str(endTime - startTime))
        wind += 1

def preProcessData():
    def util(string: str):        
        try:
            return string[0:string.index(".")+4]
        except:
            return string
    lines = list()
    for dir, subdirs, files in os.walk(rootDir):
        for file in files:
            if(file.endswith(".txt")):
                file = open(os.path.join(dir + file), 'r')
                lines.extend(file.readlines())
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

    player1list.sort(key = lambda x: (x[2], x[0]))
    player2list.sort(key = lambda x: (x[2], x[0]))

    print("Processing completed!", len(lines) - (len(player1list)+len(player2list)), "duplicates removed.\n")

    return player1list, player2list

"""Used to reduce the file size below 100MB so it can still be uploaded to gitub."""
def truncate():
    starttime = datetime.datetime.now().replace(microsecond=0)
    print("\nStarting to truncate data...")
    print("Start time: ",starttime)
   
    file = open(shotHistoryFile, 'r')
    lines = file.readlines()
    #file.truncate
    file.close()
    lines.remove(lines[0])    
    lines.remove(lines[0])
    file = open(shotHistoryFile, 'w')
    for i in range(0, len(lines)):
        size = os.path.getsize(shotHistoryFile)
        if(size < 100000000):
            file.write(lines[len(lines) - 1 - i])
        else: break
    file.close()
    
    endtime = datetime.datetime.now().replace(microsecond=0)
    print("End time: ",endtime)
    print("Total time: ", endtime-starttime)


if __name__ == "__main__":
    main()
