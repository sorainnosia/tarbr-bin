# tarbr

## Description
Tool to combine files/folder into a single tarball file and compressed it using Brotli/GZip. Support of Zip compression that combine files/folders into one file and uses Zip compression without tarball.

Has feature to split the output file into multiple files using Size or Count.

```
Please specify :
   -TB <filename> <directory> ....           Tarball list of <filename> <directory> and Brotli Compress
   -UNTB <filename> <directory> ....         Decompress brotli tarball and Extract it
   -SPLITCOUNT <filename> <count>            Splitting <filename> by count with equal size
   -SPLITSIZE <filename> <eg:1KB, 1MB, 1GB>  Splitting <filename> by size
   -JOIN <filename>                          Joining <filename>.1 <filename>.2 etc into <filename>
   -SHA <filename>                           SHA 256 hash a <filename>
   -BR <filename>                            Brotli compress <filename>
   -UNBR <filename>                          Brotli uncompress <filename>
   -GZ <filename>                            GZip compress <filename>
   -UNGZ <filename>                          GZip uncompress <filename>
   -TAR <filename> <directory> ....          Tarball list of <filename> <directory> into a single file
   -UNTAR <filename> <directory> ....        Extract tarball into a list of <filename> <directory>
   -ZIP <filename> <directory> ....          Zip list of <filename> and <directory> into a single file
   -UNZIP <filename> <directory> ....        Extract zip into a list of <filename> <directory>
Extra options
   -O <output>                               Specifying the output filename for each of the option
   -F                                        Force overwrite existing files for each of the option
General info
   -I                                        Program information

Chaining : Tar folders and files (-TAR and -F force overwrite) output to a filename (-O), brotli compress (-BR)
   the tar file (if without specifying output filename default to <filename>.br) (-F force overwrite) and split
   it into 1MB per file, resulting in final files : '<filename>.1', '<filename>.2' etc
Chaining may generate temporary file that is not needed, eg below result in 'output.tar', 'output.tar.br' are
   temp files. Output files that are required is 'output.tar.br.1', 'output.tar.br.2' etc

   -TAR <folder1> <folder2> <file1> <file2> -F -O output.tar -BR -O output.tar.br -F -SPLITSIZE 1MB

Joining 'output.tar.br.1', 'output.tar.br.2' (etc) back into 'output.tar.br' and uncompressed it to name
   'output.tar' and untar it

   -JOIN output.tar.br -F -UNBR -O output.tar -F -UNTAR

TAR folders and files and compress with brotli

   -TAR <folder1> <folder2> <file1> <file2> -F -O output.tar -BR -F -O output.tar.br

Uncompress file and extract the tar

   -UNBR output.tar.br -O output.tar -F -UNTAR

Compression and decompression for Brotli using -BR and -UNBR. For GZip compression use -GZ and -UNGZ
Chaining is also available for GZip compression algorithm. For Zip format, it includes packaging
multiple files into a single compressed file, it is not available for compression chaining and only
available to be chained for -SPLITSIZE or -SPLITCOUNT
```
