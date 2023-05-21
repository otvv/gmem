// gmem-sharp - simple memory reading/writing library for linux in C#
//
// by: otvv
// license: MIT
//

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GMem
{
  struct ProcInfo
  {
    public ProcInfo() {}

    private Int32 _processId = -1;
    private IntPtr _baseAddress = 0x0;

    public Int32 ProcessId
    {
      get { return _processId; }
      set { _processId = value; }
    }
    public IntPtr BaseAddress
    {
      get { return _baseAddress; }
      set { _baseAddress = value; }
    }
  }
  public class Proc
  {
    private ProcInfo _procInfo;

    public Proc()
    {
      // initialize ProcInfo struct
      _procInfo = new ProcInfo();
    }

    public Int32 GetProcessId(String processName)
    {
      // in case the user provides an invalid process name, dont proceed
      if (processName.Length == 0)
      {
        return -1;
      }

      // iterate the process list and look for the process name supplied by the user
      foreach (Process process in Process.GetProcessesByName(processName))
      {
        if (process.ProcessName.ToLower() == processName.ToLower()) {
          // store the found process id
          _procInfo.ProcessId = process.Id;
  
          return process.Id;
        }
      }

      return -1;
    }
    public IntPtr GetBaseAddr(String moduleName, Int32 pid)
    {
      // iterate through modules list of the process
      // and look for the module name supplied by the user
      foreach (ProcessModule module in Process.GetProcessById(pid).Modules)
      {
        if (module.ModuleName.ToLower() == moduleName.ToLower())
        {
          return module.BaseAddress;
        }
      }

      return nint.Zero;
    }
    public unsafe T ReadMem<T>(IntPtr address) where T : unmanaged
    {
      // check operating system
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
      {
        byte* buffer = stackalloc byte[sizeof(T)];

        IoVec local = new IoVec
        {
          iov_base = buffer,
          iov_len = sizeof(T)
        };
        IoVec remote = new IoVec
        {
          iov_base = address.ToPointer(),
          iov_len = sizeof(T)
        };

        process_vm_readv(_procInfo.ProcessId, &local, 1, &remote, 1, 0);

        return *(T*)buffer; 
      }

      throw new SystemException("[gmem] - invalid operating system!");
    }
    public unsafe void WriteMem<T>(IntPtr address, T value) where T : unmanaged
    {
      // check operating system
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
      {
        IoVec local = new IoVec
        {
          iov_base = &value,
          iov_len = sizeof(T)
        };
        IoVec remote = new IoVec
        {
          iov_base = address.ToPointer(),
          iov_len = sizeof(T)
        };

        process_vm_writev(_procInfo.ProcessId, &local, 1, &remote, 1, 0);
      }

      throw new SystemException("[gmem] - invalid operating system!");
    }

    [StructLayout(LayoutKind.Sequential)]
    unsafe private struct IoVec
    {
      public void* iov_base;
      public int iov_len;
    }

    [DllImport("libc")]
    private static extern unsafe int process_vm_readv(int pid, IoVec* local_iov, ulong liovcnt, IoVec* remote_iov, ulong riovcnt, ulong flags);
    [DllImport("libc")]
    private static extern unsafe int process_vm_writev(int pid, IoVec* local_iov, ulong liovcnt, IoVec* remote_iov, ulong riovcnt, ulong flags);
  }
} // namespace gmem
