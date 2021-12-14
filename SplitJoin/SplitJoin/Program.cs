using ArrayUtility;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Tar;
using System.IO.Compression;

namespace SplitJoin
{
    class SJTask
    {
        public string Operation { get; set; } = string.Empty;
        public bool ForceReplace { get; set; } = false;
        public string[] Input { get; set; } = new string[0];
        public string[] Output { get; set; } = new string[0];
        public string[] Password { get; set; } = new string[0];
        public string[] SpecificFile { get; set; } = new string[0];
    }

    class Program
    {
        static int maxmemory = 1024 * 1024;
        static int filebuffer = 1024 * 1024;

        static void Main(string[] args)
        {
            if (args != null && args.Length == 1 && args[0].ToUpper() == "-I")
            {
                Console.WriteLine("Program author                               John Kenedy");
                Console.WriteLine("Github profile                               https://github.com/sorainnosia");
                Console.WriteLine("Email                                        johnkenedypay1984@gmail.com");
                return;
            }

            if (args == null || args.Length <= 1)
            {
                Console.WriteLine("Please specify switch :");
                Console.WriteLine("   -TB <filename> <directory> ....           Tarball list of <filename> <directory> and Brotli Compress");
                Console.WriteLine("   -UNTB <filename> <directory> ....         Decompress Brotli tarball and Extract it");
                Console.WriteLine("   -TG <filename> <directory> ....           Tarball list of <filename> <directory> and GZip Compress");
                Console.WriteLine("   -UNTG <filename> <directory> ....         Decompress GZip tarball and Extract it");
                Console.WriteLine("   -TBZ <filename> <directory> ....          Tarball list of <filename> <directory> and BZip2 Compress");
                Console.WriteLine("   -UNTBZ <filename> <directory> ....        Decompress BZip2 tarball and Extract it");
                Console.WriteLine("   -TZ <filename> <directory> ....           Tarball list of <filename> <directory> and ZLib Compress");
                Console.WriteLine("   -UNTZ <filename> <directory> ....         Decompress ZLib tarball and Extract it");
                Console.WriteLine("   -SPLITCOUNT <filename> <count>            Splitting <filename> by count with equal size");
                Console.WriteLine("   -SPLITSIZE <filename> <eg:1KB, 1MB, 1GB>  Splitting <filename> by size");
                Console.WriteLine("   -JOIN <filename>                          Joining <filename>.1 <filename>.2 etc into <filename>");
                Console.WriteLine("   -CRC <filename>                           CRC checksum of a <filename>");
                Console.WriteLine("   -BR <filename>                            Brotli compress <filename>");
                Console.WriteLine("   -UNBR <filename>                          Brotli decompress <filename>");
                Console.WriteLine("   -GZ <filename>                            GZip compress <filename>");
                Console.WriteLine("   -UNGZ <filename>                          GZip decompress <filename>");
                Console.WriteLine("   -BZ2 <filename>                           BZip2 compress <filename>");
                Console.WriteLine("   -UNBZ2 <filename>                         BZip2 decompress <filename>");
                Console.WriteLine("   -ZLIB <filename>                          ZLib compress <filename>");
                Console.WriteLine("   -UNZLIB <filename>                        ZLib decompress <filename>");
                Console.WriteLine("   -TAR <filename> <directory> ....          Tarball list of <filename> <directory> into a single file");
                Console.WriteLine("   -UNTAR <directory>                        Extract tarball into a directory");
                Console.WriteLine("   -ZIP <filename> <directory> ....          Zip list of <filename> and <directory> into a single file");
                Console.WriteLine("   -UNZIP <directory>                        Extract zip into a directory");
                Console.WriteLine("   -7Z <filename> <directory> ....           7Zip list of <filename> and <directory> into a single file");
                Console.WriteLine("   -UN7Z <directory>                         Extract 7Zip into a directory");
                Console.WriteLine("Extra options per switch");
                Console.WriteLine("   -O <output>                               Specifying the output file/directory name for the switch");
                Console.WriteLine("   -F                                        Force overwrite existing files for the switch");
                Console.WriteLine("   -P <password>                             Providing password to compress/decompress for Zip and 7Zip");
                Console.WriteLine("   -SF <file1> <file2> <file3>               For -UNTAR -UNZIP and -UN7Z to extract specific file");
                Console.WriteLine("General info");
                Console.WriteLine("   -I                                        Program information");
                Console.WriteLine("");
                Console.WriteLine("Chaining : Tar folders and files to a single file (-TAR and -F force overwrite) output to filename (-O)");
                Console.WriteLine("Brotli compress (-BR) the tar file (if without specifying output filename default to <filename>.br)");
                Console.WriteLine("(-F force overwrite) and split it into 1MB per file, resulting in final files : '<filename>.1',");
                Console.WriteLine("'<filename>.2', etc. Chaining may generate temporary file that is not needed, eg below result in");
                Console.WriteLine("'output.tar', 'output.tar.br' are temp files. Output files that are required is :");
                Console.WriteLine("'output.tar.br.1', 'output.tar.br.2' etc");
                Console.WriteLine("");
                Console.WriteLine("   -TAR <folder1> <folder2> <file1> -F -O output.tar -BR -O output.tar.br -F -SPLITSIZE 1MB");
                Console.WriteLine("");
                Console.WriteLine("Joining 'output.tar.br.1', 'output.tar.br.2' (etc) back into 'output.tar.br' and decompressed it");
                Console.WriteLine("   to 'output.tar' and untar it");
                Console.WriteLine("");
                Console.WriteLine("   -JOIN output.tar.br -F -UNBR -O output.tar -F -UNTAR");
                Console.WriteLine("");
                Console.WriteLine("TAR folders and files and compress to brotli file (-BR), for GZip -GZ, BZip2 -BZ2, ZLib -ZLIB");
                Console.WriteLine("");
                Console.WriteLine("   -TAR <folder1> <folder2> <file1> <file2> -F -O output.tar -BR -F -O output.tar.br");
                Console.WriteLine("");
                Console.WriteLine("or shortform to tarball and compress -TB (brotli) (for Gzip use -TG, for BZip2 use -TBZ for ZLib use");
                Console.WriteLine("-TZ):");
                Console.WriteLine("");
                Console.WriteLine("   -TB <folder1> <folder2> <file1> <file2> -F -O output.tar.br");
                Console.WriteLine("");
                Console.WriteLine("Decompress brotli file (-UNBR) and extract the tar, for GZip -UNGZ, BZip2 -UNBZ2, ZLib -UNZLIB ");
                Console.WriteLine("");
                Console.WriteLine("   -UNBR output.tar.br -O output.tar -F -UNTAR");
                Console.WriteLine("");
                Console.WriteLine("or shortform to decompress and extract -UNTB (brotli) (for GZip use -UNTG, for BZip2 use -UNTBZ");
                Console.WriteLine("for ZLib use -UNTZ) :");
                Console.WriteLine("");
                Console.WriteLine("   -UNTB output.tar.br -O output");
                Console.WriteLine("");
                Console.WriteLine("Compression and decompression for Brotli using -BR and -UNBR. For GZip use -GZ and -UNGZ, for BZip2");
                Console.WriteLine("use -BZ and -UNBZ and for ZLib use -ZLIB and -UNZLIB");
                Console.WriteLine("Chaining is also available for GZip/ZLib/BZip2 compression algorithm. For Zip/7Zip format, it already");
                Console.WriteLine("has packaging files together with compression, it is not available for tarball and only available to be");
                Console.WriteLine("chained for -SPLITSIZE or -SPLITCOUNT");
                return;
            }

            SJTask[] ops = ParseArgs(args);

            SJTask previous = null;
            foreach (SJTask task in ops)
            {
                if (previous == null || previous.Operation == "-CRC" || previous.Operation == "-SPLITCOUNT" || previous.Operation == "-SPLITSIZE" || previous.Operation == "-UNTAR" || previous.Operation == "-UNZIP" || previous.Operation == "-UNTB" || previous.Operation == "-UNTBZ" || previous.Operation == "-UNTG" || previous.Operation == "-UNTZ" || previous.Operation == "-UN7Z")
                {
                    if ((task.Operation == "-SPLITCOUNT" || task.Operation == "-SPLITSIZE") &&
                        task.Input.Length < 2 && previous.Output.Length > 1)
                    {
                        Console.WriteLine("Please specify :");
                        if (task.Operation == "-SPLITCOUNT") Console.WriteLine(task.Operation + " <filename> <count>");
                        else Console.WriteLine(task.Operation + " <filename> <eg:1KB, 1MB, 1GB>");
                        return;
                    }
                }
                previous = task;
            }

            try
            {
                previous = null;
                foreach (SJTask task in ops)
                {
                    string input = string.Empty;
                    string output = string.Empty;

                    int afterinput = 1;
                    if (task.Operation != "-SPLITCOUNT" && task.Operation != "-SPLITSIZE")
                    {
                        if (task.Input.Length > 0) input = task.Input[0];
                        else if (previous != null)
                        {
                            input = previous?.Output?[0] ?? "";
                            task.Input = previous.Output;
                        }
                    }
                    else
                    {
                        if (task.Input.Length == 1 && previous != null)
                        {
                            input = previous?.Output?[0] ?? "";
                            if (previous.Output.Length > 0)
                            {
                                task.Input = ArrayUtil.AddArray(previous.Output[0], task.Input);
                                afterinput = 1;
                            }
                            else
                            {
                                task.Input = previous.Output;
                                afterinput = 1;
                            }
                        }
                        else if (task.Input.Length > 1)
                        {
                            input = task.Input[0];
                            afterinput = 1;
                        }
                    }
                    if (task?.Output?.Length > 0) output = task.Output[0];

                    switch (task?.Operation)
                    {
                        case "-CRC":
                            string sum = GetChecksum(input);
                            if (string.IsNullOrEmpty(sum) == false)
                                Console.WriteLine("CRC of '" + TrimCurrentDir(input) + "': " + sum.ToLower());
                            task.Output = new string[] { sum };
                            break;
                        case "-BR":
                            CompressedUnCompress("BR", task, task.Input, output, task.ForceReplace, new Func<string, string, bool, string>(Br));
                            break;
                        case "-GZ":
                            CompressedUnCompress("GZ", task, task.Input, output, task.ForceReplace, new Func<string, string, bool, string>(Gz));
                            break;
                        case "-BZ2":
                            CompressedUnCompress("BZ2", task, task.Input, output, task.ForceReplace, new Func<string, string, bool, string>(Bz2));
                            break;
                        case "-ZLIB":
                            CompressedUnCompress("ZLIB", task, task.Input, output, task.ForceReplace, new Func<string, string, bool, string>(ZLib));
                            break;
                        case "-UNBR":
                            CompressedUnCompress("UNBR", task, task.Input, output, task.ForceReplace, new Func<string, string, bool, string>(UnBr));
                            break;
                        case "-UNGZ":
                            CompressedUnCompress("UNGZ", task, task.Input, output, task.ForceReplace, new Func<string, string, bool, string>(UnGz));
                            break;
                        case "-UNBZ2":
                            CompressedUnCompress("UNBZ2", task, task.Input, output, task.ForceReplace, new Func<string, string, bool, string>(UnBz2));
                            break;
                        case "-UNZLIB":
                            CompressedUnCompress("UNZLIB", task, task.Input, output, task.ForceReplace, new Func<string, string, bool, string>(UnZLib));
                            break;
                        case "-JOIN":
                            CompressedUnCompress("JOIN", task, task.Input, output, task.ForceReplace, new Func<string, string, bool, string>(Join));
                            break;
                        case "-TB":
                            ShortForm("br", task, input, output, new Func<string, string, bool, string>(Br));
                            break;
                        case "-TBZ":
                            ShortForm("bz2", task, input, output, new Func<string, string, bool, string>(Bz2));
                            break;
                        case "-TG":
                            ShortForm("gz", task, input, output, new Func<string, string, bool, string>(Gz));
                            break;
                        case "-TZ":
                            ShortForm("zlib", task, input, output, new Func<string, string, bool, string>(ZLib));
                            break;
                        case "-UNTZ":
                            UnShortForm("zlib", task, input, output, new Func<string, string, bool, string>(UnZLib));
                            break;
                        case "-UNTB":
                            UnShortForm("br", task, input, output, new Func<string, string, bool, string>(UnBr));
                            break;
                        case "-UNTBZ":
                            UnShortForm("bz2", task, input, output, new Func<string, string, bool, string>(UnBz2));
                            break;
                        case "-UNTG":
                            UnShortForm("gz", task, input, output, new Func<string, string, bool, string>(UnGz));
                            break;
                        case "-TAR":
                            string tar = Tar(task.Input, output, task.ForceReplace);
                            if (string.IsNullOrEmpty(tar) == false)
                                Console.WriteLine("TAR of '" + TrimCurrentDir(input) + "' (...etc): " + TrimCurrentDir(tar));
                            task.Output = new string[] { tar };
                            break;
                        case "-UNTAR":
                            string[] resulttar = new string[0];
                            string untar = UnTar(input, output, task.SpecificFile, task.ForceReplace, out resulttar);
                            if (string.IsNullOrEmpty(untar) == false)
                                Console.WriteLine("UNTAR of '" + TrimCurrentDir(input) + "': " + TrimCurrentDir(untar));
                            task.Output = resulttar;
                            break;
                        case "-ZIP":
                            string zip = Zip(task.Input, output, task.ForceReplace, string.Join(" ", task.Password));
                            if (string.IsNullOrEmpty(zip) == false)
                                Console.WriteLine("ZIP of '" + TrimCurrentDir(input) + "' (...etc): " + TrimCurrentDir(zip));
                            task.Output = new string[] { zip };
                            break;
                        case "-UNZIP":
                            string[] resultzip = new string[0];
                            string unzip = UnZip(input, output, string.Join(" ", task.Password), task.SpecificFile, task.ForceReplace, out resultzip);
                            if (string.IsNullOrEmpty(unzip) == false)
                                Console.WriteLine("UNZIP of '" + TrimCurrentDir(input) + "': " + TrimCurrentDir(unzip));
                            task.Output = resultzip;
                            break;
                        case "-7Z":
                            string _7zip = _7Zip(task.Input, output, task.ForceReplace, string.Join(" ", task.Password));
                            if (string.IsNullOrEmpty(_7zip) == false)
                                Console.WriteLine("7Z of '" + TrimCurrentDir(input) + "' (...etc): " + TrimCurrentDir(_7zip));
                            task.Output = new string[] { _7zip };
                            break;
                        case "-UN7Z":
                            string[] result7zip = new string[0];
                            string un7zip = Un7Zip(input, output, string.Join(" ", task.Password), task.SpecificFile, task.ForceReplace, out result7zip);
                            if (string.IsNullOrEmpty(un7zip) == false)
                                Console.WriteLine("UN7Z of '" + TrimCurrentDir(input) + "': " + TrimCurrentDir(un7zip));
                            task.Output = result7zip;
                            break;
                        case "-SPLITSIZE":
                        case "-SPLITCOUNT":

                            int count = 0;
                            int size = 0;
                            if (task.Input.Length < 2)
                            {
                                Console.WriteLine(task.Operation.Substring(1) + ": Please specify ");
                                if (task.Operation == "-SPLITCOUNT")
                                    Console.WriteLine(task.Operation + " <filename> <count>");
                                else
                                    Console.WriteLine(task.Operation + " <filename> <eg:1KB, 1MB, 1GB>");
                                continue;
                            }
                            if (task.Operation == "-SPLITCOUNT")
                            {
                                int.TryParse(task.Input[afterinput], out count);
                                if (count <= 1)
                                {
                                    Console.WriteLine("-SPLITCOUNT: Split Count must be greater than 1 chunk");
                                    continue;
                                }
                            }
                            else
                            {
                                if (task.Input[afterinput].Length <= 2)
                                {
                                    Console.WriteLine("-SPLITSIZE: Split Size must be amount[unit] (eg: 1KB, 1MB, 1GB)");
                                    continue;
                                }

                                string unit = task.Input[afterinput].Substring(task.Input[afterinput].Length - 2);
                                string sz = task.Input[afterinput].Substring(0, task.Input[afterinput].Length - 2);

                                int.TryParse(sz, out size);
                                unit = unit.ToUpper();
                                if (unit == "KB") size = size * 1024;
                                else if (unit == "MB") size = size * 1024 * 1024;
                                else if (unit == "GB") size = size * 1024 * 1024 * 1024;

                                if (size < 1024)
                                {
                                    Console.WriteLine("-SPLITSIZE: Split Size must be greater or equal than 1KB");
                                    continue;
                                }
                            }
                            string[] resultsplit = Split(input, count, size, output, task.ForceReplace);
                            if (resultsplit != null && resultsplit.Length != 0)
                                Console.WriteLine(task.Operation.Substring(1) + " of '" + TrimCurrentDir(input) + "': '" + TrimCurrentDir(resultsplit[0]) + "' until '" + TrimCurrentDir(resultsplit[resultsplit.Length - 1]) + "'");
                            task.Output = resultsplit;
                            break;
                    }
                    if (task.Output == null || task.Output.Length == 0 || string.IsNullOrEmpty(task.Output[0]))
                    {
                        //Console.WriteLine(task.Operation.Substring(1) + ": error");
                        return;
                    }
                    previous = task;
                }
            }
            catch (ICSharpCode.SharpZipLib.Zip.ZipException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            catch (InvalidDataException ex)
            {
                if (ex.Message.IndexOf("File is not a") >= 0)
                {
                    Console.WriteLine("Exception: Invalid archive");
                }
                else
                {
                    Console.WriteLine("Exception: Invalid Password");
                }
            }
            catch (OutOfMemoryException)
            {
                Console.WriteLine("Not enough memory, program terminated");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found error, program terminated");
            }
            catch (AccessViolationException)
            {
                Console.WriteLine("Program is unable to create/overwrite the file because its in used or locked");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Program is unable to create/overwrite the file because its in used or locked");
            }
            catch (IOException)
            {
                Console.WriteLine("Exception: Not enough disk space or file is locked or invalid archive");
            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }

        #region Utility

        private static SJTask[] ParseArgs(string[] args)
        {
            if (args == null || args.Length == 0) return new SJTask[0];
            SJTask[] result = new SJTask[0];
            string[] ops = new string[] { "-TAR", "-UNTAR", "-BR", "-UNBR", "-SPLITCOUNT", "-SPLITSIZE", "-JOIN", "-CRC", "-ZIP", "-UNZIP", "-GZ", "-UNGZ", "-ZLIB", "-UNZLIB", "-BZ2", "-UNBZ2", "-TB", "-UNTB", "-TBZ", "-UNTBZ", "-TG", "-UNTG", "-TZ", "-UNTZ", "-7Z", "-UN7Z" };

            bool output = false;
            bool password = false;
            bool specificfile = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (ArrayUtil.ArrayContains(ops, args[i].ToUpper()))
                {
                    SJTask obj = new SJTask();
                    obj.Operation = args[i].ToUpper();
                    result = ArrayUtil.AddArray(result, obj);
                }
                else if (args[i].ToUpper() == "-O")
                {
                    output = true;
                    password = false;
                    specificfile = false;
                }
                else if (args[i].ToUpper() == "-P")
                {
                    password = true;
                    output = false;
                    specificfile = false;
                }
                else if (args[i].ToUpper() == "-SF")
                {
                    specificfile = true;
                    output = false;
                    password = false;
                }
                else if (args[i].ToUpper() == "-F")
                {
                    if (result.Length == 0)
                    {
                        Console.WriteLine("Error: Invalid option -F without specifying task");
                        return new SJTask[0];
                    }
                    result[result.Length - 1].ForceReplace = true;
                }
                else
                {
                    if (result.Length == 0)
                    {
                        Console.WriteLine("Error: Invalid option, run program without parameter to see list of options");
                        return new SJTask[0];
                    }

                    if (output)
                    {
                        result[result.Length - 1].Output = new string[] { args[i] };
                        output = false;
                        password = false;
                        specificfile = false;
                    }
                    else if (password)
                    {
                        result[result.Length - 1].Password = ArrayUtil.AddArray(result[result.Length - 1].Password, args[i]);
                        password = false;
                        output = false;
                        specificfile = false;
                    }
                    else if (specificfile)
                    {
                        result[result.Length - 1].SpecificFile = ArrayUtil.AddArray(result[result.Length - 1].SpecificFile, args[i].Replace("\\", "/"));
                        password = false;
                        output = false;
                        specificfile = false;
                    }
                    else
                    {
                        result[result.Length - 1].Input = ArrayUtil.AddArray(result[result.Length - 1].Input, args[i]);
                    }
                }
            }
            return result;
        }

        private static bool IsAlreadyExist(string type, string file, bool replace)
        {
            if (File.Exists(file) && replace)
                File.Delete(file);
            else if (File.Exists(file))
            {
                Console.WriteLine(type + ": File '" + TrimCurrentDir(file) + "' already exists");
                return true;
            }
            return false;
        }
        #endregion

        #region ShortForm
        private static void ShortForm(string type, SJTask task, string input, string output, Func<string, string, bool, string> func)
        {
            string o2 = output;
            if (string.IsNullOrEmpty(o2)) o2 = task.Input[0] + ".tar";
            else o2 = o2 + ".tar";

            string tar = Tar(task.Input, o2, task.ForceReplace);
            if (string.IsNullOrEmpty(tar) == false)
                Console.WriteLine("TAR of '" + TrimCurrentDir(task.Input[0]) + "' (...etc) : " + TrimCurrentDir(tar));
            task.Output = new string[] { tar };

            if (string.IsNullOrEmpty(tar) == false)
            {
                string o3 = tar + "." + type;
                if (IsAlreadyExist(type, o3, task.ForceReplace)) return;

                string shorten = func(tar, o3, task.ForceReplace);
                if (string.IsNullOrEmpty(shorten) == false)
                    Console.WriteLine(type.ToUpper() + " of '" + TrimCurrentDir(tar) + "' : " + TrimCurrentDir(shorten));
                task.Output = new string[] { shorten };
            }
        }

        private static void UnShortForm(string type, SJTask task, string input, string output, Func<string, string, bool, string> func)
        {
            string o2 = input;
            if (o2.ToUpper().EndsWith("." + type.ToUpper()) && o2.Length > type.Length + 1) o2 = o2.Substring(0, o2.Length - (type.Length + 1));

            if (IsAlreadyExist(type, o2, task.ForceReplace)) return;
            string unshort = func(input, o2, task.ForceReplace);
            if (string.IsNullOrEmpty(unshort) == false)
                Console.WriteLine("UN" + type.ToUpper() + " of '" + TrimCurrentDir(input) + "': " + TrimCurrentDir(unshort));
            task.Output = new string[] { unshort };

            string o3 = unshort;
            if (o3.ToUpper().EndsWith(".TAR") && o3.Length > 4) o3 = o3.Substring(0, o2.Length - 4);
            if (string.IsNullOrEmpty(output) == false) o3 = output;
            if (IsAlreadyExist(type, o3, task.ForceReplace)) return;

            if (string.IsNullOrEmpty(unshort) == false)
            {
                string[] result = new string[0];
                string untar = UnTar(unshort, o3, task.SpecificFile, task.ForceReplace, out result);
                if (string.IsNullOrEmpty(untar) == false)
                    Console.WriteLine("UNTAR of '" + TrimCurrentDir(unshort) + "': " + TrimCurrentDir(untar));
                task.Output = new string[] { untar };
            }
        }
        #endregion

        #region Tar
        private static string Tar(string[] input, string output, bool replace)
        {
            string ot = Path.GetFullPath(input[0]);
            ot = ot + ".tar";
            if (string.IsNullOrEmpty(output) == false) ot = output;
            string dir = Path.GetDirectoryName(ot);
            CreateDir(dir);
            if (IsAlreadyExist("TAR", ot, replace)) return string.Empty;

            using (var outStream = File.Create(ot))
            {
                using (var tarArchive = TarArchive.CreateOutputTarArchive(outStream))
                {
                    for (int i = 0; i < input.Length; i++)
                    {
                        if (Directory.Exists(input[i]))
                        {
                            AddTarDir(tarArchive, Path.GetDirectoryName(input[i]) ?? "", Path.GetFileName(input[i]));
                        }
                        else
                        {
                            var tarEntry = TarEntry.CreateEntryFromFile(input[i]);
                            tarEntry.Name = Path.GetFileName(input[i]);
                            tarArchive.WriteEntry(tarEntry, true);
                        }
                    }
                    tarArchive.Close();
                }
            }
            return ot;
        }

        private static string UnTar(string input, string output, string[] specificfile, bool replace, out string[] result)
        {
            result = new string[0];
            if (File.Exists(input) == false)
            {
                Console.WriteLine("UNTAR: File '" + TrimCurrentDir(input) + "' does not exists");
                return string.Empty;
            }

            string destFolder = Path.GetFullPath(input);
            if (destFolder.ToUpper().EndsWith(".TAR") && destFolder.Length > 4)
                destFolder = destFolder.Substring(0, destFolder.Length - 4);
            destFolder = RemoveExtension(destFolder);

            if (string.IsNullOrEmpty(output) == false) destFolder = output;
            if (File.Exists(destFolder) == false && Directory.Exists(destFolder) == false) Directory.CreateDirectory(destFolder);

            int count = 0;
            int failed = 0;
            int tried = 0;
            using (Stream inStream = new FileStream(input, FileMode.Open, FileAccess.Read, FileShare.Read, filebuffer, FileOptions.None))
            {
                using (TarInputStream inputStream = new TarInputStream(inStream, System.Text.Encoding.Default))
                {
                    TarEntry entry = inputStream.GetNextEntry();
                    while (entry != null)
                    {
                        if (specificfile == null || specificfile.Length == 0 || ArrayUtil.ArrayContains(specificfile, entry.Name.Replace("\\", "/")))
                        {
                            try
                            {
                                string filePath = Path.Combine(destFolder, entry.Name);
                                if (entry.TarHeader.TypeFlag != TarHeader.LF_DIR)
                                {
                                    tried++;
                                    string dir2 = Path.GetDirectoryName(filePath);
                                    CreateDir(dir2);
                                    if (File.Exists(filePath) && replace) File.Delete(filePath);

                                    using (Stream oustream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.Write, filebuffer, FileOptions.None))
                                    {
                                        inputStream.CopyTo(oustream, filebuffer);
                                    }
                                    result = ArrayUtil.AddArray(result, filePath);
                                    count++;
                                }
                                else
                                {
                                    tried++;
                                    CreateDir(filePath);
                                    count++;
                                }
                            }
                            catch (Exception ex)
                            {
                                failed++;
                                Console.WriteLine("UNTAR: Error file '" + TrimCurrentDir(entry.Name) + "' : " + ex.Message);
                            }
                            if (specificfile != null && specificfile.Length > 0 && specificfile.Length == tried)
                                break;
                        }
                        entry = inputStream.GetNextEntry();
                    }
                }
            }
            Console.WriteLine("UNTAR: Success " + count + " Failed " + failed);
            return count > 0 ? destFolder : "";
        }

        private static void AddTarDir(TarArchive tarArchive, string sourceDirectory, string currentdirectory)
        {
            var pathToCurrentDirectory = Path.Combine(sourceDirectory, currentdirectory);
            if (string.IsNullOrEmpty(sourceDirectory))
            {
                sourceDirectory = currentdirectory;
            }

            var filePaths = Directory.GetFiles(pathToCurrentDirectory);
            if (filePaths != null && filePaths.Length > 0)
            {
                foreach (string filePath in filePaths)
                {
                    var tarEntry = TarEntry.CreateEntryFromFile(filePath);
                    if (IsRoot(sourceDirectory))
                        tarEntry.Name = filePath.Substring(sourceDirectory.Length).TrimStart(new char[] { '/', '\\' });
                    else
                        tarEntry.Name = filePath;
                    tarArchive.WriteEntry(tarEntry, true);
                }
            }
            else
            {
                DirectoryInfo diri = new DirectoryInfo(pathToCurrentDirectory);

                string name = "";
                if (IsRoot(sourceDirectory))
                    name = pathToCurrentDirectory.Substring(sourceDirectory.Length).TrimStart(new char[] { '/', '\\' });
                else
                    name = pathToCurrentDirectory;

                TarHeader header;
                header = new TarHeader();
                header.Name = name;

                header.ModTime = diri.CreationTime;
                header.Mode = 511;
                header.TypeFlag = TarHeader.LF_DIR;
                header.Size = 0;
                var tarEntry = new TarEntry(header);

                tarArchive.WriteEntry(tarEntry, true);
            }

            var directories = Directory.GetDirectories(pathToCurrentDirectory);
            foreach (string directory in directories)
            {
                AddTarDir(tarArchive, sourceDirectory, directory.Substring(sourceDirectory.Length + 1));
            }
        }
        #endregion

        #region Zip
        private static void AddZipDir(ICSharpCode.SharpZipLib.Zip.ZipOutputStream zipArchive, string sourceDirectory, string currentdirectory)
        {
            var pathToCurrentDirectory = Path.Combine(sourceDirectory, currentdirectory);
            if (string.IsNullOrEmpty(sourceDirectory))
            {
                sourceDirectory = currentdirectory;
            }

            var filePaths = Directory.GetFiles(pathToCurrentDirectory);
            if (filePaths != null && filePaths.Length > 0)
            {
                foreach (string filePath in filePaths)
                {
                    ICSharpCode.SharpZipLib.Zip.ZipEntry zipEntry = null;
                    if (IsRoot(sourceDirectory))
                        zipEntry = new ICSharpCode.SharpZipLib.Zip.ZipEntry(filePath.Substring(sourceDirectory.Length).TrimStart(new char[] { '/', '\\' })); // zipArchive.CreateEntry(filePath.Substring(sourceDirectory.Length).TrimStart(new char[] { '/', '\\' }));
                    else
                        zipEntry = new ICSharpCode.SharpZipLib.Zip.ZipEntry(filePath);// zipArchive.CreateEntry(filePath);

                    zipArchive.PutNextEntry(zipEntry);
                    using (Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, filebuffer, FileOptions.None))
                    {
                        //using (var entryStream = zipEntry.Open())
                        stream.CopyTo(zipArchive, filebuffer);
                    }
                }
            }
            else
            {
                ICSharpCode.SharpZipLib.Zip.ZipEntry zipEntry = null;
                zipEntry = new ICSharpCode.SharpZipLib.Zip.ZipEntry(pathToCurrentDirectory.TrimEnd(new char[] { '\\' }) + "\\");
                zipArchive.PutNextEntry(zipEntry);
                zipArchive.CloseEntry();
            }

            var directories = Directory.GetDirectories(pathToCurrentDirectory);
            foreach (string directory in directories)
            {
                AddZipDir(zipArchive, sourceDirectory, directory.Substring(sourceDirectory.Length + 1));
            }
        }

        private static string Zip(string[] input, string output, bool replace, string password)
        {
            string ot = Path.GetFullPath(input[0]);
            ot = ot + ".zip";
            if (string.IsNullOrEmpty(output) == false) ot = output;
            string dir = Path.GetDirectoryName(ot);
            CreateDir(dir);
            if (IsAlreadyExist("ZIP", ot, replace)) return string.Empty;

            using (var outStream = File.Create(ot))
            {
                using (var zipArchive = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(outStream, filebuffer))
                {
                    if (string.IsNullOrEmpty(password) == false) zipArchive.Password = password;
                    for (int i = 0; i < input.Length; i++)
                    {
                        if (Directory.Exists(input[i]))
                        {
                            AddZipDir(zipArchive, Path.GetDirectoryName(input[i]) ?? "", Path.GetFileName(input[i]));
                        }
                        else
                        {
                            //var fileInArchive = zipArchive.CreateEntry(Path.GetFileName(input[i]));
                            ICSharpCode.SharpZipLib.Zip.ZipEntry entry = new ICSharpCode.SharpZipLib.Zip.ZipEntry(Path.GetFileName(input[i]));
                            zipArchive.PutNextEntry(entry);
                            using (Stream stream = new FileStream(input[i], FileMode.Open, FileAccess.Read, FileShare.Read, filebuffer, FileOptions.None))
                            {
                                stream.CopyTo(zipArchive, filebuffer);
                            }
                        }
                    }
                    zipArchive.Finish();
                }
            }
            return ot;
        }

        private static string UnZip(string input, string output, string password, string[] specificfile, bool replace, out string[] result)
        {
            result = new string[0];
            if (File.Exists(input) == false)
            {
                Console.WriteLine("UNZIP: File '" + TrimCurrentDir(input) + "' does not exists");
                return string.Empty;
            }

            string destFolder = Path.GetFullPath(input);
            if (destFolder.ToUpper().EndsWith(".ZIP") && destFolder.Length > 4)
                destFolder = destFolder.Substring(0, destFolder.Length - 4);
            destFolder = RemoveExtension(destFolder);

            if (string.IsNullOrEmpty(output) == false) destFolder = output;
            if (File.Exists(destFolder) == false && Directory.Exists(destFolder) == false) Directory.CreateDirectory(destFolder);

            //ZipFile.ExtractToDirectory(input, destFolder);

            bool resultx = ExtractZipDir(input, destFolder, password, specificfile, replace, out result);

            return resultx ? destFolder : "";
        }

        private static bool ExtractZipDir(string file, string destinationdirectory, string password, string[] specificfile, bool replace, out string[] result)
        {
            result = new string[0];
            var pathToCurrentDirectory = destinationdirectory;

            int count = 0;
            int failed = 0;
            int tried = 0;
            using (var outStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, filebuffer))
            {
                using (var zipArchive = new ICSharpCode.SharpZipLib.Zip.ZipInputStream(outStream, filebuffer))
                {
                    ICSharpCode.SharpZipLib.Zip.ZipEntry zipEntry = zipArchive.GetNextEntry();

                    while (zipEntry != null)
                    {
                        if (zipEntry.IsCrypted && string.IsNullOrEmpty(password) == false) zipArchive.Password = password;

                        if (specificfile == null || specificfile.Length == 0 || ArrayUtil.ArrayContains(specificfile, zipEntry.Name.Replace("\\", "/")))
                        {
                            try
                            {
                                var filePath = Path.Combine(destinationdirectory, zipEntry.Name);
                                if (zipEntry.IsDirectory == false)
                                {
                                    tried++;
                                    string dir2 = Path.GetDirectoryName(filePath);
                                    CreateDir(dir2);
                                    if (File.Exists(filePath) && replace) File.Delete(filePath);

                                    using (Stream stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.Write, filebuffer, false))
                                    {
                                        zipArchive.CopyTo(stream, filebuffer);
                                    }
                                    result = ArrayUtil.AddArray(result, filePath);
                                    count++;
                                }
                                else
                                {
                                    tried++;
                                    CreateDir(filePath);
                                    count++;
                                }
                            }
                            catch (Exception ex)
                            {
                                failed++;
                                Console.WriteLine("UNZIP: Error file '" + TrimCurrentDir(zipEntry.Name) + "' : " + ex.Message);
                            }
                            if (specificfile != null && specificfile.Length > 0 && specificfile.Length == tried)
                                break;
                        }
                        zipEntry = zipArchive.GetNextEntry();
                    }
                }
            }
            Console.WriteLine("UNZIP: Success " + count + " Failed " + failed);
            return count > 0 ? true : false;
        }
        #endregion

        #region 7Zip
        private static async Task Add7ZipDir(ManagedLzma.SevenZip.Writer.ArchiveMetadataRecorder metadata, ManagedLzma.SevenZip.Writer.EncoderSession session, string sourceDirectory, string currentdirectory)
        {
            var pathToCurrentDirectory = Path.Combine(sourceDirectory, currentdirectory);
            if (string.IsNullOrEmpty(sourceDirectory))
            {
                sourceDirectory = currentdirectory;
            }

            var filePaths = Directory.GetFiles(pathToCurrentDirectory);
            if (filePaths != null && filePaths.Length > 0)
            {
                foreach (string filePath in filePaths)
                {
                    FileInfo file = new FileInfo(filePath);
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, filebuffer))
                    {
                        var result = await session.AppendStream(fileStream, true);
                        string name = file.Name;
                        if (IsRoot(sourceDirectory))
                            name = filePath.Substring(sourceDirectory.Length).TrimStart(new char[] { '/', '\\' });
                        else
                            name = filePath;

                        metadata.AppendFile(name, result.Length, result.Checksum, file.Attributes, file.CreationTimeUtc, file.LastWriteTimeUtc, file.LastAccessTimeUtc);
                    }
                }
            }
            else
            {
                string name = pathToCurrentDirectory;
                DirectoryInfo di = new DirectoryInfo(name);
                if (IsRoot(sourceDirectory))
                    name = pathToCurrentDirectory.Substring(sourceDirectory.Length).TrimStart(new char[] { '/', '\\' });
                else
                    name = pathToCurrentDirectory;

                //using (FileStream fs = new FileStream(name, FileMode.Open, FileAccess.Read, FileShare.Read, filebuffer))
                //{
                //var result = session.AppendStream(null, false);
                //metadata.AppendDirectory(name.TrimEnd(new char[] { '\\' }) + "\\", FileAttributes.Directory, di.CreationTime, di.LastWriteTime, di.LastAccessTime);
                //}
            }

            var directories = Directory.GetDirectories(pathToCurrentDirectory);
            foreach (string directory in directories)
            {
                await Add7ZipDir(metadata, session, sourceDirectory, directory.Substring(sourceDirectory.Length + 1));
            }
        }

        private static string _7Zip(string[] input, string output, bool replace, string password)
        {
            string ot = Path.GetFullPath(input[0]);
            ot = ot + ".7z";
            if (string.IsNullOrEmpty(output) == false) ot = output;
            string dir = Path.GetDirectoryName(ot);
            CreateDir(dir);
            if (IsAlreadyExist("7Z", ot, replace)) return string.Empty;

            ot = Task.Run(async delegate
            {
                using (var archiveStream = new FileStream(ot, FileMode.Create, FileAccess.ReadWrite, FileShare.Delete, filebuffer))
                using (var archiveWriter = ManagedLzma.SevenZip.Writer.ArchiveWriter.Create(archiveStream, false))
                {
                    var encoder = new ManagedLzma.SevenZip.Writer.EncoderDefinition();
                    ManagedLzma.SevenZip.Writer.EncoderNodeDefinition node1 = null;
                    ManagedLzma.SevenZip.Writer.EncoderNodeDefinition node2 = null;
                    ManagedLzma.PasswordStorage pass = null;
                    if (string.IsNullOrEmpty(password) == false) pass = ManagedLzma.PasswordStorage.Create(password);

                    //node1 = encoder.CreateEncoder(new ManagedLzma.SevenZip.Writer.Lzma2EncoderSettings(new ManagedLzma.LZMA2.EncoderSettings()));
                    if (string.IsNullOrEmpty(password) == false)
                    {
                        node2 = encoder.CreateEncoder(new ManagedLzma.SevenZip.Writer.AesEncoderSettings(pass));
                    }
                    else
                    {
                        //node2 = encoder.CreateEncoder(new ManagedLzma.SevenZip.Writer.AesEncoderSettings(ManagedLzma.PasswordStorage.Create("")));
                        node2 = encoder.CreateEncoder(new ManagedLzma.SevenZip.Writer.LzmaEncoderSettings(new ManagedLzma.LZMA.EncoderSettings()));
                    }

                    if (node1 != null && node2 != null)
                    {
                        encoder.Connect(encoder.GetContentSource(), node1.GetInput(0));
                        encoder.Connect(node1.GetOutput(0), node2.GetInput(0));
                        encoder.Connect(node2.GetOutput(0), encoder.CreateStorageSink());
                    }
                    else
                    {
                        encoder.Connect(encoder.GetContentSource(), (node1 ?? node2).GetInput(0));
                        encoder.Connect((node1 ?? node2).GetOutput(0), encoder.CreateStorageSink());
                    }
                    encoder.Complete();

                    var metadata = new ManagedLzma.SevenZip.Writer.ArchiveMetadataRecorder();
                    using (var session = archiveWriter.BeginEncoding(encoder, true))
                    {
                        for (int i = 0; i < input.Length; i++)
                        {
                            if (Directory.Exists(input[i]))
                            {
                                await Add7ZipDir(metadata, session, Path.GetDirectoryName(input[i]) ?? "", Path.GetFileName(input[i]));
                            }
                            else if (File.Exists(input[i]))
                            {
                                FileInfo file = new FileInfo(input[i]);
                                using (FileStream fileStream = new FileStream(input[i], FileMode.Open, FileAccess.Read, FileShare.Read, filebuffer))
                                {
                                    var result = await session.AppendStream(fileStream, true);
                                    metadata.AppendFile(file.Name, result.Length, result.Checksum, file.Attributes, file.CreationTimeUtc, file.LastWriteTimeUtc, file.LastAccessTimeUtc);
                                }
                            }
                        }
                        await session.Complete();
                    }
                    await archiveWriter.WriteMetadata(metadata);
                    await archiveWriter.WriteHeader();
                    return ot;
                }
            }).GetAwaiter().GetResult();

            return ot;
        }

        private static string Un7Zip(string input, string output, string password, string[] specificfile, bool replace, out string[] result)
        {
            result = new string[0];
            if (File.Exists(input) == false)
            {
                Console.WriteLine("UN7Z: File '" + TrimCurrentDir(input) + "' does not exists");
                return string.Empty;
            }

            string destFolder = Path.GetFullPath(input);
            if (destFolder.ToUpper().EndsWith(".7Z") && destFolder.Length > 3)
                destFolder = destFolder.Substring(0, destFolder.Length - 3);
            destFolder = RemoveExtension(destFolder);

            if (string.IsNullOrEmpty(output) == false) destFolder = output;
            if (File.Exists(destFolder) == false && Directory.Exists(destFolder) == false) Directory.CreateDirectory(destFolder);

            master._7zip.Legacy.Password pass = null;
            if (string.IsNullOrEmpty(password) == false) pass = new master._7zip.Legacy.Password(password);
            var db = new master._7zip.Legacy.CArchiveDatabaseEx();
            var x = new master._7zip.Legacy.ArchiveReader();

            int count = 0;
            int failed = 0;
            int tried = 0;
            using (Stream fs = new FileStream(input, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete))
            {
                x.Open(fs);
                x.ReadDatabase(db, pass);
                db.Fill();
                x.Extract(db, null, pass);
                var files = x.GetFiles(db);
                var fenum = files.GetEnumerator();
                int i = 0;

                while (fenum.MoveNext())
                {
                    master._7zip.Legacy.CFileItem file = fenum.Current;

                    if (specificfile == null || specificfile.Length == 0 || ArrayUtil.ArrayContains(specificfile, file.Name.Replace("\\", "/")))
                    {
                        try
                        {
                            var filePath = Path.Combine(destFolder, file.Name);
                            if (file.HasStream)
                            {
                                tried++;
                                string dir2 = Path.GetDirectoryName(filePath);
                                CreateDir(dir2);
                                if (File.Exists(filePath) && replace) File.Delete(filePath);

                                using (Stream str = x.OpenStream(db, i, pass))
                                {
                                    using (Stream outstream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, filebuffer))
                                    {
                                        str.CopyTo(outstream, filebuffer);
                                    }
                                }
                                result = ArrayUtil.AddArray(result, filePath);
                                count++;
                            }
                            else
                            {
                                tried++;
                                CreateDir(filePath);
                                count++;
                            }
                        }
                        catch (Exception ex)
                        {
                            failed++;
                            Console.WriteLine("UN7Z: Error file '" + TrimCurrentDir(file.Name) + "' : " + ex.Message);
                        }
                        if (specificfile != null && specificfile.Length > 0 && specificfile.Length == tried)
                            break;
                    }
                    i++;
                }
            }
            if (master._7zip.Legacy.CArchiveDatabaseEx.MustDelete != null && master._7zip.Legacy.CArchiveDatabaseEx.MustDelete.Length > 0)
            {
                try
                {
                    foreach (string str in master._7zip.Legacy.CArchiveDatabaseEx.MustDelete)
                        File.Delete(str);
                }
                catch (Exception)
                { }
            }
            Console.WriteLine("UN7Z: Success " + count + " Failed " + failed);
            return count > 0 ? destFolder : string.Empty;
        }

        #endregion

        #region Single Uncompress
        private static string UnBr(string file, string output, bool replace)
        {
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, filebuffer, FileOptions.None))
            using (var ms = new FileStream(output, FileMode.CreateNew, FileAccess.Write, FileShare.Write, filebuffer))
            {
                using (var bs = new BrotliStream(fs, CompressionMode.Decompress))
                {
                    bs.CopyTo(ms, filebuffer);
                }
            }

            return output;
        }

        private static string UnGz(string file, string output, bool replace)
        {
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, filebuffer, FileOptions.None))
            using (var ms = new FileStream(output, FileMode.CreateNew, FileAccess.Write, FileShare.Write, filebuffer))
            {
                using (var bs = new GZipStream(fs, CompressionMode.Decompress))
                {
                    bs.CopyTo(ms, filebuffer);
                }
            }

            return output;
        }

        private static string UnZLib(string file, string output, bool replace)
        {
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, filebuffer, FileOptions.None))
            using (var ms = new FileStream(output, FileMode.CreateNew, FileAccess.Write, FileShare.Write, filebuffer))
            {
                using (var bs = new ZLibStream(fs, CompressionMode.Decompress))
                {
                    bs.CopyTo(ms, filebuffer);
                }
            }

            return output;
        }

        private static string UnBz2(string file, string output, bool replace)
        {
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, filebuffer, FileOptions.None))
            using (var ms = new FileStream(output, FileMode.CreateNew, FileAccess.Write, FileShare.Write, filebuffer))
            {
                using (var bs = new BZip2InputStream(fs))
                {
                    bs.CopyTo(ms, filebuffer);
                }
            }

            return output;
        }
        #endregion

        #region Single Compress
        private static string[] AddCompressDir(string type, string sourceDirectory, string currentdirectory, string[] input, string output, bool replace, Func<string, string, bool, string> func, ref int count, ref int failed, ref int tried)
        {
            var pathToCurrentDirectory = Path.Combine(sourceDirectory, currentdirectory);
            if (string.IsNullOrEmpty(sourceDirectory))
            {
                sourceDirectory = currentdirectory;
            }

            string[] result = new string[0];
            var filePaths = Directory.GetFiles(pathToCurrentDirectory);
            if (filePaths != null && filePaths.Length > 0)
            {
                foreach (string filePath in filePaths)
                {
                    string name = filePath;
                    if (IsRoot(sourceDirectory))
                        name = filePath.Substring(sourceDirectory.Length).TrimStart(new char[] { '/', '\\' });
                    else
                        name = filePath;

                    try
                    {
                        tried++;

                        string ot = Path.GetFullPath(filePath);
                        if (type.StartsWith("UN"))
                        {
                            if (ot.Length > (type.Length - 2) - 1 && ot.ToUpper().EndsWith("." + type.Substring(2).ToUpper()))
                                ot = ot.Substring(0, ot.Length - (type.Length - 2) - 1);
                            else
                                ot = RemoveExtension(ot);
                        }
                        else
                            ot = ot + "." + type.ToLower();

                        if (input.Length == 1 && filePaths.Length == 1 && string.IsNullOrEmpty(output) == false) ot = output;
                        else if (type == "JOIN" && input.Length > 0 && input[0].EndsWith(".1")) ot = output;

                        string dir = Path.GetDirectoryName(ot);
                        CreateDir(dir);
                        if (IsAlreadyExist(type.ToUpper(), ot, replace))
                        {
                            failed++;
                            continue;
                        }

                        string str = string.Empty;
                        if (type.ToUpper() == "JOIN")
                        {
                            string fl = filePath;
                            if (filePaths.Length > 1 && fl.EndsWith(".1") && fl.Length > 2)
                                fl = fl.Substring(0, fl.LastIndexOf("."));
                            str = func(fl, fl, replace);
                            if (string.IsNullOrEmpty(str) == false) result = ArrayUtil.AddArray(result, str);
                            count++;
                            break;
                        }
                        else
                        {
                            str = func(filePath, ot, replace);
                            if (string.IsNullOrEmpty(str) == false) result = ArrayUtil.AddArray(result, str);
                        }
                        count++;
                    }
                    catch (Exception ex)
                    {
                        failed++;
                        Console.WriteLine(type.ToUpper() + ": Error file '" + TrimCurrentDir(filePath) + "', " + ex.Message);
                    }
                }
            }

            var directories = Directory.GetDirectories(pathToCurrentDirectory);
            foreach (string directory in directories)
            {
                string[] result2 = AddCompressDir(type, sourceDirectory, directory.Substring(sourceDirectory.Length + 1), input, output, replace, func, ref count, ref failed, ref tried);
                if (result2 != null && result2.Length > 0) result = ArrayUtil.AddArray(result, result2);
            }
            return result;
        }

        public static string CompressedUnCompress(string type, SJTask task, string[] input, string output, bool replace, Func<string, string, bool, string> func)
        {
            string[] result = new string[0];

            int count = 0;
            int failed = 0;
            int tried = 0;
            string resultstr = string.Empty;
            for (int i = 0; i < input.Length; i++)
            {
                if (Directory.Exists(input[i]))
                {
                    string[] temp = AddCompressDir(type, Path.GetDirectoryName(input[i]) ?? "", Path.GetFileName(input[i]), input, output, replace, func, ref count, ref failed, ref tried);
                    if (temp != null && temp.Length > 0)
                        result = ArrayUtil.AddArray(result, temp);
                }
                else if (File.Exists(input[i]))
                {
                    try
                    {
                        tried++;

                        string ot = Path.GetFullPath(input[i]);
                        if (type.StartsWith("UN"))
                        {
                            if (ot.ToUpper().EndsWith("." + type.Substring(2).ToUpper()) && ot.Length > (type.Length - 2) - 1)
                                ot = ot.Substring(0, ot.Length - (type.Length - 2) - 1);
                            else
                                ot = RemoveExtension(ot);
                        }
                        else
                            ot = ot + "." + type.ToLower();
                        if (input.Length == 1 && string.IsNullOrEmpty(output) == false) ot = output;
                        else if (type == "JOIN" && input.Length > 0 && input[0].EndsWith(".1")) ot = output;


                        string dir = Path.GetDirectoryName(ot);
                        CreateDir(dir);
                        if (IsAlreadyExist(type.ToUpper(), ot, replace))
                        {
                            failed++;
                            continue;
                        }

                        if (type.ToUpper() == "JOIN")
                        {
                            string fl = input[i];
                            if (input.Length > 1 && fl.EndsWith(".1") && input.Length > 2)
                                fl = fl.Substring(0, fl.LastIndexOf("."));
                            resultstr = func(fl, fl, replace);
                            if (string.IsNullOrEmpty(resultstr) == false) result = ArrayUtil.AddArray(result, resultstr);
                            count++;
                            break;
                        }
                        else
                        {
                            resultstr = func(input[i], ot, replace);
                            if (string.IsNullOrEmpty(resultstr) == false) result = ArrayUtil.AddArray(result, resultstr);
                        }
                        count++;
                    }
                    catch (Exception ex)
                    {
                        failed++;
                        Console.WriteLine(type.ToUpper() + ": Error file '" + TrimCurrentDir(input[i]) + "', " + ex.Message);
                    }
                }
                else
                {
                    failed++;
                    Console.WriteLine(type.ToUpper() + ": Error file '" + TrimCurrentDir(input[i]) + "' does not exists");
                }
            }

            if (count == 1 && (input.Length == 1 || type.ToUpper() == "JOIN"))
                Console.WriteLine(type.ToUpper() + " of '" + TrimCurrentDir(input[0]) + "': '" + TrimCurrentDir(resultstr) + "'");
            else
                Console.WriteLine(type.ToUpper() + ": Success " + count + " Failed " + failed);

            task.Output = result;
            return count > 0 ? resultstr : string.Empty;
        }

        private static string Br(string file, string output, bool replace)
        {
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, filebuffer, FileOptions.None))
            using (var ms = new FileStream(output, FileMode.CreateNew, FileAccess.Write, FileShare.Write, filebuffer))
            {
                using (BrotliStream bs = new BrotliStream(ms, CompressionLevel.Fastest))
                {
                    fs.CopyTo(bs, filebuffer);
                }
            }

            return output;
        }

        private static string Bz2(string file, string output, bool replace)
        {
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, filebuffer, FileOptions.None))
            using (var ms = new FileStream(output, FileMode.CreateNew, FileAccess.Write, FileShare.Write, filebuffer))
            {
                using (BZip2OutputStream bs = new BZip2OutputStream(ms))
                {
                    fs.CopyTo(bs, filebuffer);
                }
            }

            return output;
        }

        private static string ZLib(string file, string output, bool replace)
        {
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, filebuffer, FileOptions.None))
            using (var ms = new FileStream(output, FileMode.CreateNew, FileAccess.Write, FileShare.Write, filebuffer))
            {
                using (ZLibStream bs = new ZLibStream(ms, CompressionMode.Compress))
                {
                    fs.CopyTo(bs, filebuffer);
                }
            }

            return output;
        }

        private static string Gz(string file, string output, bool replace)
        {
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, filebuffer, FileOptions.None))
            using (var ms = new FileStream(output, FileMode.CreateNew, FileAccess.Write, FileShare.Write, filebuffer))
            {
                using (GZipStream bs = new GZipStream(ms, CompressionMode.Compress))
                {
                    fs.CopyTo(bs, filebuffer);
                }
            }

            return output;
        }
        #endregion

        private static string GetChecksum(string file)
        {
            if (File.Exists(file) == false)
            {
                Console.WriteLine("SHA: File '" + file + "' does not exists");
                return string.Empty;
            }

            //Sha256Digest myHash = new Sha256Digest();
            //using (BufferedStream stream = new BufferedStream(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, filebuffer, FileOptions.None), 1024 * 1024))
            //{
            //    byte[] buffer = new byte[4096];
            //    int cbSize;
            //    while ((cbSize = stream.Read(buffer, 0, buffer.Length)) > 0)
            //        myHash.BlockUpdate(buffer, 0, cbSize);
            //}

            //byte[] compArr = new byte[myHash.GetDigestSize()];
            //myHash.DoFinal(compArr, 0);

            FileInfo fi = new FileInfo(file);
            using (Stream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, filebuffer, FileOptions.None))
            {
                uint i = ManagedLzma.CRC.From(fs, fi.Length);
                return i.ToString("X");
            }
        }

        private static string[] Split(string filename, long count, long size, string output, bool replace)
        {
            if (File.Exists(filename) == false)
            {
                Console.WriteLine("SPLITSIZE/SPLITCOUNT: File '" + TrimCurrentDir(filename) + "' does not exists");
                return null;
            }

            FileInfo fi = new FileInfo(filename);
            long length = fi.Length;
            long byteperchunk = 0;
            long remain = 0;
            if (count != 0)
            {
                byteperchunk = length / count;
                remain = length % byteperchunk;
                if (remain == 0) remain = byteperchunk;
                size = byteperchunk;
            }
            else
            {
                count = length / size;
                if (length % size != 0) count++;
                byteperchunk = size;
                remain = length % size;
                if (remain == 0) remain = size;
            }

            string ot = Path.GetFullPath(filename);
            if (string.IsNullOrEmpty(output) == false) ot = output;
            string dir = Path.GetDirectoryName(ot);
            CreateDir(dir);

            string[] result = new string[0];
            using (BinaryReader br = new BinaryReader(new FileStream(filename, FileMode.Open)))
            {
                for (int i = 0; i < count; i++)
                {
                    long chunk = byteperchunk;
                    if (i == count - 1) chunk = remain;

                    string chunkname = ot + "." + (i + 1).ToString();
                    if (File.Exists(chunkname) && replace) File.Delete(chunkname);
                    using (BinaryWriter bw = new BinaryWriter(new FileStream(chunkname, FileMode.CreateNew)))
                    {
                        long portion = chunk / maxmemory;
                        if (chunk % maxmemory != 0) portion += 1;

                        long remainx = chunk;

                        for (int j = 0; j < portion; j++)
                        {
                            if (remainx >= maxmemory)
                            {
                                bw.Seek(0, SeekOrigin.End);
                                byte[] buffer2 = br.ReadBytes(maxmemory);
                                bw.Write(buffer2);
                                remainx -= maxmemory;
                            }
                            else
                            {
                                bw.Seek(0, SeekOrigin.End);
                                byte[] buffer2 = br.ReadBytes((int)remainx);
                                bw.Write(buffer2);
                                remainx = 0;
                            }
                        }
                        result = ArrayUtil.AddArray(result, ot + "." + (i + 1).ToString());
                    }
                }
                if (replace)
                {
                    string chunkname = ot + "." + (count + 1).ToString();
                    while (File.Exists(chunkname))
                    {
                        try
                        {
                            File.Delete(chunkname);
                        }
                        catch { }
                        count++;
                    }
                }
            }
            return result;
        }

        public static string RemoveExtension(string destFolder)
        {
            while (destFolder.IndexOf(".") > 0 && destFolder.LastIndexOf(".") >= destFolder.Length - 5)
                destFolder = destFolder.Substring(0, destFolder.LastIndexOf("."));
            return destFolder;
        }

        private static bool IsRoot(string dir)
        {
            if (string.IsNullOrEmpty(dir)) return false;
            return dir.Contains(":");
        }

        private static string TrimCurrentDir(string path)
        {
            string curr = Directory.GetCurrentDirectory().TrimEnd(new char[] { '\\', '/' }) + Path.DirectorySeparatorChar;
            if (path.ToLower().StartsWith(curr.ToLower()))
                path = path.Substring(curr.Length);
            return path;
        }

        private static void CreateDir(string dir)
        {
            try
            {
                if (string.IsNullOrEmpty(dir) == false && dir != "/" && dir != "\\" && dir != "~" && File.Exists(dir) == false && Directory.Exists(dir) == false)
                    Directory.CreateDirectory(dir);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: Can't create directory '" + dir + "', " + ex.Message);
                throw;
            }
        }

        private static string Join(string input, string output, bool replace)
        {
            string filename = input;

            string ot = Path.GetFullPath(filename);
            if (string.IsNullOrEmpty(output) == false) ot = output;
            string dir = Path.GetDirectoryName(ot);
            CreateDir(dir);
            if (IsAlreadyExist("JOIN", ot, replace)) return string.Empty;

            if (input.Length == 1 && File.Exists(filename + ".1") == false)
            {
                Console.WriteLine("JOIN: First file chunk does not exist with filename : '" + TrimCurrentDir(filename) + ".1'");
                return string.Empty;
            }

            int chunk = 1;
            using (BinaryWriter sw = new BinaryWriter(new FileStream(ot, FileMode.Append)))
            {
                while (File.Exists(filename + "." + chunk.ToString()))
                {
                    sw.Seek(0, SeekOrigin.End);
                    FileInfo fi = new FileInfo(filename + "." + chunk.ToString());
                    long length = fi.Length;
                    using (BinaryReader br = new BinaryReader(new FileStream(filename + "." + chunk, FileMode.Open)))
                    {
                        byte[] buffer = new byte[maxmemory];
                        while (length > 0)
                        {
                            if (length > maxmemory)
                            {
                                buffer = br.ReadBytes(maxmemory);
                                sw.Write(buffer, 0, maxmemory);
                                length -= maxmemory;
                            }
                            else
                            {
                                buffer = br.ReadBytes((int)length);
                                sw.Write(buffer, 0, (int)length);
                                length = 0;
                            }
                        }
                    }
                    chunk++;
                }
            }
            return ot;
        }
    }
}
