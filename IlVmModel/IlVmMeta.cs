namespace IlVmModel
{
    //The IL VM should consume a byte array (fixed, list or as a stream) which contains all the data it needs to execute the code.
    //  string, int constants can be serailized inline.
    //  locals will all be initialized and set by the VM so only need to serailize the type info
    //  args
    //  methods -> locally, references to methods, pointers to functions, handles or metadata token s
    public class IlVmMeta // when building dynamic types/methods store strings,ints,locals,arguments, and methods
    {
        //alternately a single array[] of object would be simpler, simply store object reference pointers.
        //  but serailizing over the wire becomes a problem.
        public string[] strings; // serializes inline
        public int[] ints; // serializes inline
        public object[] locals; // serailization becomes tricky here
        public object[] args; // serailization becomes tricky here
        public object[] methods; // serailize by store information need to load after being sent across the wire.
    }
}

