using anoncreds_rs_dotnet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace anoncreds_rs_dotnet.Models
{
    public static class Structures
    {
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct FfiStrList
        {
            public IntPtr count;
            public FfiStr* data;
            public static FfiStrList Create(string[] args)
            {
                FfiStrList list = new FfiStrList();
                if (args != null && args.Any())
                {
                    list.count = (IntPtr)args.Length;
                    if (args.First() != null)
                    {
                        FfiStr[] ffiStrings = new FfiStr[(uint)args.Length];
                        for (int i = 0; i < args.Length; i++)
                        {
                            ffiStrings[i] = FfiStr.Create(args[i]);
                        }
                        fixed (FfiStr* ffiStr_p = &ffiStrings[0])
                        {
                            list.data = ffiStr_p;
                        }
                    }
                }
                else
                {
                    list.count = IntPtr.Zero;
                    list.data = null;
                }
                return list;
            }

            public static FfiStrList Create(List<string> args)
            {
                if (args != null)
                {
                    return Create(args.ToArray());
                }
                else
                {
                    return Create(new List<string>());
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FfiStr
        {
            public IntPtr data;

            public static FfiStr Create(string arg)
            {
                FfiStr FfiString = new FfiStr()
                {
                    data = new IntPtr()
                };
                if (arg != null)
                {
                    FfiString.data = Marshal.StringToCoTaskMemAnsi(arg);
                }
                return FfiString;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct ByteBuffer
        {
            public long len;
            public IntPtr value;

            public static ByteBuffer Create(string json)
            {
                ByteBuffer buffer = new ByteBuffer();
                if (!string.IsNullOrEmpty(json))
                {
                    UTF8Encoding decoder = new UTF8Encoding(true, true);
                    var byteCount = decoder.GetByteCount(json);
                    byte[] bytes = new byte[byteCount];
                    buffer.len = decoder.GetBytes(json, 0, json.Length, bytes, 0);
                    fixed (byte* bytebuffer_p = &bytes[0])
                    {
                        buffer.value = new IntPtr(bytebuffer_p);
                    }
                }
                else
                {
                    buffer.len = 0;
                    buffer.value = new IntPtr();
                }
                return buffer;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct FfiCredRevInfo
        {
            public IntPtr regDefObjectHandle;
            public IntPtr regDefPvtObjectHandle;
            public IntPtr regIdx;
            public FfiStr tailsPath;

            internal static FfiCredRevInfo Create(Models.CredentialRevocationConfig entry)
            {
                FfiCredRevInfo result = new FfiCredRevInfo();

                result.regDefObjectHandle = entry.RevRegDefObjectHandle;
                result.regDefPvtObjectHandle = entry.RevRegDefPvtObjectHandle;
                result.regIdx = (IntPtr)entry.RegIdx;
                result.tailsPath = FfiStr.Create(entry.TailsPath);

                return result;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct FfiLongList
        {
            public IntPtr count;
            public long* data;
            public static FfiLongList Create(long[] args = null)
            {
                FfiLongList list = new FfiLongList();
                if (args != null)
                {
                    list.count = (IntPtr)args.Length;
                    fixed (long* uintP = &args[0])
                    {
                        list.data = uintP;
                    }
                }
                else
                {
                    list.count = IntPtr.Zero;
                    list.data = (long*)0;
                }
                return list;
            }

            public static FfiLongList Create(List<long> args)
            {
                if (args != null)
                {
                    return Create(args.ToArray());
                }
                else
                {
                    return Create();
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FfiCredentialEntry
        {
            public IntPtr CredentialObjectHandle;
            public long Timestamp;
            public IntPtr RevStateObjectHandle;

            public static FfiCredentialEntry Create(CredentialEntry entry)
            {
                FfiCredentialEntry result = new FfiCredentialEntry();
                if (entry != null)
                {
                    result.CredentialObjectHandle = entry.CredentialObjectHandle;
                    result.Timestamp = entry.Timestamp;
                    result.RevStateObjectHandle = entry.RevStateObjectHandle;
                }
                return result;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FfiCredentialProof
        {
            public long EntryIndex;
            public FfiStr Referent;
            public byte IsPredicate;
            public byte Reveal;

            public static FfiCredentialProof Create(CredentialProof prove)
            {
                FfiCredentialProof result = new FfiCredentialProof()
                {
                    EntryIndex = prove.EntryIndex,
                    Referent = FfiStr.Create(prove.Referent),
                    IsPredicate = prove.IsPredicate,
                    Reveal = prove.Reveal
                };
                return result;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FfiRevocationEntry
        {
            public long DefEntryIdx;
            public IntPtr Entry;
            public long Timestamp;

            public static FfiRevocationEntry Create(RevocationRegistryEntry entry)
            {
                FfiRevocationEntry result = new FfiRevocationEntry()
                {
                    DefEntryIdx = entry.DefEntryIdx,
                    Entry = entry.Entry,
                    Timestamp = entry.Timestamp
                };

                return result;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FfiNonrevokedIntervalOverride
        {
            public FfiStr RevRegDefId;
            public int RequestedFromTs;
            public int OverrideRevStatusListTs;

            public static FfiNonrevokedIntervalOverride Create(NonrevokedIntervalOverride intervalOverride)
            {
                FfiNonrevokedIntervalOverride result = new FfiNonrevokedIntervalOverride()
                {
                    RevRegDefId = FfiStr.Create(intervalOverride.RevRegDefId),
                    RequestedFromTs = intervalOverride.RequestedFromTs,
                    OverrideRevStatusListTs = intervalOverride.OverrideRevStatusListTs
                };

                return result;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct FfiCredentialEntryList
        {
            public IntPtr count;
            public FfiCredentialEntry* data;

            public static FfiCredentialEntryList Create(CredentialEntry[] args)
            {
                FfiCredentialEntryList list = new FfiCredentialEntryList();

                if (args != null && args.Any())
                {
                    list.count = (IntPtr)args.Length;
                    FfiCredentialEntry[] ffiCredentialEntries = new FfiCredentialEntry[args.Length];
                    for (int i = 0; i < args.Length; i++)
                    {
                        ffiCredentialEntries[i] = FfiCredentialEntry.Create(args[i]);
                    }
                    fixed (FfiCredentialEntry* ffiEntryP = &ffiCredentialEntries[0])
                    {
                        list.data = ffiEntryP;
                    }
                }
                else
                {
                    list.count = IntPtr.Zero;
                    list.data = null;
                }

                return list;
            }

            public static FfiCredentialEntryList Create(List<CredentialEntry> args)
            {
                if (args != null)
                {
                    return Create(args.ToArray());
                }
                else
                {
                    return Create(new List<CredentialEntry>());
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct FfiCredentialProveList
        {
            public IntPtr count;
            public FfiCredentialProof* data;

            public static FfiCredentialProveList Create(CredentialProof[] args)
            {
                FfiCredentialProveList list = new FfiCredentialProveList();

                if (args != null && args.Any())
                {
                    list.count = (IntPtr)args.Length;
                    FfiCredentialProof[] ffiCredentialProves = new FfiCredentialProof[args.Length];
                    for (int i = 0; i < args.Length; i++)
                    {
                        ffiCredentialProves[i] = FfiCredentialProof.Create(args[i]);
                    }
                    fixed (FfiCredentialProof* ffiProveP = &ffiCredentialProves[0])
                    {
                        list.data = ffiProveP;
                    }
                }
                else
                {
                    list.count = IntPtr.Zero;
                    list.data = null;
                }

                return list;
            }

            public static FfiCredentialProveList Create(List<CredentialProof> args)
            {
                if (args != null)
                {
                    return Create(args.ToArray());
                }
                else
                {
                    return Create(new List<CredentialProof>());
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct FfiUIntList
        {
            public IntPtr count;
            public IntPtr* data;

            public static FfiUIntList Create(IntPtr[] args)
            {
                FfiUIntList list = new FfiUIntList();

                if (args != null && args.Any())
                {
                    list.count = (IntPtr)args.Length;
                    fixed (IntPtr* uintP = &args[0])
                    {
                        list.data = uintP;
                    }
                }
                else
                {
                    list.count = IntPtr.Zero;
                    list.data = null;
                }

                return list;
            }

            public static FfiUIntList Create(List<IntPtr> args)
            {
                if (args != null)
                {
                    return Create(args.ToArray());
                }
                else
                {
                    return Create(new List<IntPtr>());
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct FfiRevocationEntryList
        {
            public IntPtr count;
            public FfiRevocationEntry* data;
            public static FfiRevocationEntryList Create(RevocationRegistryEntry[] args = null)
            {
                FfiRevocationEntryList list = new FfiRevocationEntryList();

                if (args != null && args.Any())
                {
                    list.count = (IntPtr)args.Length;
                    FfiRevocationEntry[] ffiRevocationEntries = new FfiRevocationEntry[args.Length];
                    for (int i = 0; i < args.Length; i++)
                    {
                        ffiRevocationEntries[i] = FfiRevocationEntry.Create(args[i]);
                    }
                    fixed (FfiRevocationEntry* ffiEntryP = &ffiRevocationEntries[0])
                    {
                        list.data = ffiEntryP;
                    }
                }
                else
                {
                    list.count = IntPtr.Zero;
                    list.data = null;
                }

                return list;
            }

            public static FfiRevocationEntryList Create(List<RevocationRegistryEntry> args)
            {
                if (args != null)
                {
                    return Create(args.ToArray());
                }
                else
                {
                    return Create(new List<RevocationRegistryEntry>());
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct FfiNonrevokedIntervalOverrideList
        {
            public IntPtr count;
            public FfiNonrevokedIntervalOverride* data;

            public static FfiNonrevokedIntervalOverrideList Create(NonrevokedIntervalOverride[] args)
            {
                FfiNonrevokedIntervalOverrideList list = new FfiNonrevokedIntervalOverrideList();

                if (args != null && args.Any())
                {
                    list.count = (IntPtr)args.Length;
                    FfiNonrevokedIntervalOverride[] ffiNIOs = new FfiNonrevokedIntervalOverride[args.Length];
                    for (int i = 0; i < args.Length; i++)
                    {
                        ffiNIOs[i] = FfiNonrevokedIntervalOverride.Create(args[i]);
                    }
                    fixed (FfiNonrevokedIntervalOverride* ffiNIOP = &ffiNIOs[0])
                    {
                        list.data = ffiNIOP;
                    }
                }
                else
                {
                    list.count = IntPtr.Zero;
                    list.data = null;
                }

                return list;
            }

            public static FfiNonrevokedIntervalOverrideList Create(List<NonrevokedIntervalOverride> args)
            {
                if (args != null)
                {
                    return Create(args.ToArray());
                }
                else
                {
                    return Create(new List<NonrevokedIntervalOverride>());
                }
            }
        }
    }
}