import os
import sys
import subprocess
import collections

class CreateDoc:
    def createCommandLine(self, commandLine, verb, subverb):
        """
        create the command line
        """
        return " ".join([commandLine, verb, subverb]).strip()

    def addHelpToLine(self, line):
        """
        add help to a line
        """
        return line + " --help"

    def createName(self, verb, subverb):
        """
        return an assemble of 2 capitalized names
        """
        return verb.capitalize() + subverb.capitalize()

    def runCommandLine(self, cmdLine):
        """
        run a command line
        return it's stdout
        """
        process = subprocess.run(cmdLine.split(" "),
                                check=True,
                                stdout=subprocess.PIPE,
                                stderr=subprocess.STDOUT,
                                universal_newlines=True)
        stdout = process.stdout
        return stdout

    def CommandList(self):
        """
        return commands for Windows and unix
        """
        commandList = ["qarnot", "qarnot.exe"]
        return commandList

    def parseUsage(self, usage):
        """
        get a man and split it to it's different values
        """
        newUsage = usage.splitlines()
        version = ""
        copyrightQarnot = ""
        UsageList = []
        FlagList = []
        step = 0
        for line in newUsage:
            if step == 0:
                version = line
                step += 1
            elif step == 1:
                copyrightQarnot = line
                step += 1
            elif step == 2:
                UsageList.append(line)
                if line.strip() == "":
                    step += 1
            elif step == 3:
                FlagList.append(line)
        return version, copyrightQarnot, "\n".join(UsageList), "\n".join(FlagList)

    def CreateMan(self, name, commandUnix, commandDos, usage, helpLine):
        """
        assemble the man info extracted in a dictionary
        """
        version, copyrightQarnot, usageList, flagList = self.parseUsage(usage)
        filename = name
        linesDict = collections.OrderedDict()

        linesDict["name"] = name
        linesDict["helpLine"] = helpLine
        linesDict["commandUnix"] = commandUnix
        linesDict["commandDos"] = commandDos
        linesDict["usageList"] = usageList
        linesDict["flagList"] = flagList
        linesDict["version"] = version
        linesDict["copyrightQarnot"] = copyrightQarnot

        return {"name": filename, "value": linesDict}

    def getHelpUsage(self, testCommand, verb, usageHelpList, newUsageList):
        """
        extract the command verbs from a "command help"
        """
        if verb.startswith("-"):
            return
        lineCommand = self.createCommandLine(testCommand, verb, "")
        lineHelp = self.addHelpToLine(lineCommand)
        usage = self.runCommandLine(lineHelp)
        binaryName = "qarnot"
        for line in usage.splitlines():
            if line.startswith("  ") and not line.startswith("  -") and not line.startswith("  " + binaryName):
                sublines = [l for l in line.split("  ") if l]
                if len(sublines) == 2:
                    name = "".join([w.capitalize() for w in sublines[0].split(" ")])
                    usageHelpList[name] = [sublines[0].strip(), sublines[1].strip()]
                    verb = sublines[0].strip()
                    if verb != "help" and verb != "version":
                        newUsageList.append(sublines[0].strip())
                else:
                    print("Error in the usage verb parsing")
                    print(sublines)

    def GetManFormOneCommand(self, testCommand, commandUsage, key, elem, usageHelpList):
        """
        extract and split all the info of a man
        """
        commandList = self.CommandList()
        lineCommand = self.createCommandLine(testCommand, key, elem)
        lineHelp = self.addHelpToLine(lineCommand)
        usage = self.runCommandLine(lineHelp)
        name = self.createName(key, elem)
        lineCommandUnix = self.createCommandLine(commandList[0], key, elem)
        lineCommandDos = self.createCommandLine(commandList[1], key, elem)
        commandUsage.append(self.CreateMan(name, lineCommandUnix, lineCommandDos, usage, usageHelpList[name][1]))

    def CreateAndLaunchAllUsages(self, testCommand, printer):
        """
        Get the list of command to launch
        Launch all commands with a "--help"
        Split the verbs to the flags
        """
        newUsageList = []
        subverbList = []
        commandUsage = []
        usageHelpList = dict()
        SuvVerbusageHelpList = {}
        # get the help extract the commands names to launch them and extract the help line of the verbs
        self.getHelpUsage(testCommand, "", SuvVerbusageHelpList, newUsageList)

        # stock the Command
        sinfo = {"name": "Commands", "value": SuvVerbusageHelpList}
        subverbList.append(sinfo)
        # stock again the command for the full list)
        usageHelpList.update(SuvVerbusageHelpList.copy())

        # launch all the command find
        for key in newUsageList:
            subList = []
            # idem
            SuvVerbusageHelpList = {}
            self.getHelpUsage(testCommand, key, SuvVerbusageHelpList, subList)
            usageHelpList.update(SuvVerbusageHelpList.copy())

            # diff the basic command from the commands with subcommands
            if len(subList) == 0:
                # basic command is parse for it usage
                self.GetManFormOneCommand(testCommand, commandUsage, key, "", usageHelpList)
            else:
                # get the subcommand names
                sinfo = {"name": key.capitalize(), "value": SuvVerbusageHelpList}
                subverbList.append(sinfo)
                # launch all the command + subcommand find
                for elem in subList:
                    elem = elem.split(" ")[1]
                    self.GetManFormOneCommand(testCommand, commandUsage, key, elem, usageHelpList)

        # Print the commands
        for command in commandUsage:
            printer.PrintInFile(command["name"], command["value"])

        # print the sub list
        subverbList.reverse()
        for subverb in subverbList:
            printer.PrintInFile(subverb["name"], {"subverbList": subverb})

        # print the big list
        name = "ManIndex"
        indexDict = {"beginIndex": "", "IndexList": usageHelpList}
        printer.PrintInFile(name, indexDict)


class MarkdownFormat:
    """
    class converting a dict to a string markDown format
    for the printer
    """

    def __init__(self, directory, extend):
        """
        Get the directory and extend names to test the files
        map all the functions to easly use them
        every dict function return a string
        """
        self.directory = directory
        self.extend = extend
        self.dictReturnValues = {}
        self.dictReturnValues["name"] = self.CreateName
        self.dictReturnValues["helpLine"] = self.CreateHelpLine
        self.dictReturnValues["version"] = self.CreateVersion
        self.dictReturnValues["copyrightQarnot"] = self.CreateCopyrightQarnot
        self.dictReturnValues["commandUnix"] = self.CreateCommandUnix
        self.dictReturnValues["commandDos"] = self.CreateCommandDos
        self.dictReturnValues["usageList"] = self.CreateUsageList
        self.dictReturnValues["flagList"] = self.CreateFlagList
        self.dictReturnValues["beginIndex"] = self.CreateBeginIndex
        self.dictReturnValues["IndexList"] = self.CreateIndexList
        self.dictReturnValues["subverbList"] = self.CreateSubverbList

        self.header = "# Qarnot CLI \n" + \
            "> List of the commands\n" + \
            "\n" + \
            "\n"

    # Man funections
    def CreateName(self, key, value):
        """
        Man name
        """
        return "# {1}\n".format(key, value)

    def CreateHelpLine(self, key, value):
        """
        Man small explication line
        """
        return "> {1}  \n\n".format(key, value)

    def CreateCommandUnix(self, key, value):
        """
        Man unix command
        """
        return "Unix \n```bash\n {1}\n```\n".format(key, value)

    def CreateCommandDos(self, key, value):
        """
        Man DOS command
        """
        return "Windows \n```bash\n {1}\n```\n".format(key, value)

    def CreateVersion(self, key, value):
        """
        CLI Version
        """
        return "*Version*: *{1}*  \n".format(key, value)

    def CreateCopyrightQarnot(self, key, value):
        """
        CLI Copyright
        """
        return "*Copyright*: *{1}*  \n".format(key, value)

    def CreateUsageList(self, key, value):
        """
        Usage exemples
        """
        return "***\n### {0}  {1}\n***\n".format(value.split("\n")[0], "  \n".join(["> * `" + line + "`" if line.startswith("  ") else "\n>" + line for line in value.strip().split("\n")[1:]]))

    def CreateFlagList(self, key, value):
        """
        Flag list
        """
        return "### Flags: \n```bash\n {1}\n```\n".format(key, value)

    # Table of man functions
    def CreateBeginIndex(self, key, value):
        return self.header + "### {1}\n| name | description |\n|:--:|:--|\n".format(key, value)

    def CreateValuesIndex(self, key, value):
        if key == "Help" or key == "Version":
            return ""
        if os.path.exists(os.path.join(self.directory, key + self.extend)):
            return "|[{1}]({0}{3})|{2}|\n".format(key, value[0], value[1], self.extend)
        print(os.path.join(self.directory, key + self.extend) + " not found")
        return ""

    # Command Tables
    def CreateIndexList(self, key, valueDict):
        ret = ""
        for subKey in valueDict:
            ret += self.CreateValuesIndex(subKey, valueDict[subKey])
        return ret

    # Sub command Tables
    def CreateSubverbList(self, name, value):
        # print(value)
        retString = self.CreateBeginIndex("", value["name"])
        for key in value["value"]:
            retString += self.CreateValuesIndex(key, value["value"][key])
        return retString

    def CreateFormatedString(self, key, value):
        """
        Start function
        Launch the dictionary
        or exit
        """
        if key in self.dictReturnValues:
            return self.dictReturnValues[key](key, value)
        else:
            print("\n\nvalue not found : " + key)
            print(value)
            exit(1)


class XMLFormat:
    """
    class converting dict to string format
    for the printer
    """
    def CreateFormatedString(self, key, value):
        return "<{}>\n{}\n</{}>\n".format(key, value, key)


class PrintDoc:
    """
    Class printing in the document
    """
    def FormatValue(self, key, value):
        return self.format.CreateFormatedString(key, value)

    def __init__(self, name, extention, form):
        self.extention = extention
        self.format = form
        self.dirName = name
        self.CreateDir(name)

    def CreateDir(self, dirName):
        """
        create the directory
        """
        try:
            os.makedirs(dirName)
            print("Directory ", dirName, " Created ")
        except FileExistsError:
            print("Directory ", dirName, " already exists")

    def WriteInFile(self, dirName, fileName, linesDict):
        filePath = os.path.join(dirName, fileName)
        with open(filePath, 'w') as file:
            for key in linesDict:
                value = linesDict[key]
                file.write(self.FormatValue(key, value))

    def PrintInFile(self, fileBaseName, linesDict):
        self.WriteInFile(self.dirName, fileBaseName + self.extention, linesDict)


def getCommandPath():
    """
    GEt the first elem in the command path
    Or send the usage
    """
    if len(sys.argv) > 1:
        return sys.argv[1]
    else:
        print("Usage : python3 createDoc.py [binary-path]")
        print("Example: python3 createDoc.py /usr/bin/qarnot")
        print("         python3 createDoc.py ../Realize/qarnot")
        exit(0)


def main():
    """
    Launch the main
    """
    testCommand = getCommandPath()
    directory = "manMarkDown"
    file_extend = ".md"
    form = MarkdownFormat(directory, file_extend)
    printer = PrintDoc(directory, file_extend, form)
    create = CreateDoc()
    print("start of Cli Doc extraction")
    create.CreateAndLaunchAllUsages(testCommand, printer)
    print("end of Cli Doc extraction")


if __name__ == "__main__":
    # execute only if run as a script
    main()
