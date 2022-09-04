using System.Management;
using System.Net.NetworkInformation;
using Microsoft.Win32;

namespace AliSls;

public static class DeviceUtil
{
    public static string? GetSystemId()
    {
        string systemId = null;
        using (ManagementObjectSearcher mos =
               new ManagementObjectSearcher("select * from Win32_ComputerSystemProduct"))
        {
            foreach (var item in mos.Get())
            {
                systemId = item["UUID"].ToString();
            }
        }

        return systemId;
    }

    public static string? GetMac()
    {
        var macAddr =
        (
            from nic in NetworkInterface.GetAllNetworkInterfaces()
            where nic.OperationalStatus == OperationalStatus.Up
            select nic.GetPhysicalAddress().ToString()
        ).FirstOrDefault();
        return macAddr;
    }

    public static String? GetMachineGuid()
    {
        try
        {
            string x64Result = string.Empty;
            string x86Result = string.Empty;
            //此处如果需要可以判断一下系统是什么位数的,比如32或者64,用以取分读取哪个视图,因为视图不同,获得的数据也不同
            RegistryKey keyBaseX64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            RegistryKey keyBaseX86 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            RegistryKey keyX64 = keyBaseX64.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography",
                RegistryKeyPermissionCheck.ReadSubTree);
            RegistryKey keyX86 = keyBaseX86.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography",
                RegistryKeyPermissionCheck.ReadSubTree);
            object resultObjX64 = keyX64.GetValue("MachineGuid", (object)"default");
            object resultObjX86 = keyX86.GetValue("MachineGuid", (object)"default");
            keyX64.Close();
            keyX86.Close();
            keyBaseX64.Close();
            keyBaseX86.Close();
            keyX64.Dispose();
            keyX86.Dispose();
            keyBaseX64.Dispose();
            keyBaseX86.Dispose();
            keyX64 = null;
            keyX86 = null;
            keyBaseX64 = null;
            keyBaseX86 = null;
            if (resultObjX64 != null && resultObjX64.ToString() != "default")
            {
                return resultObjX64.ToString();
            }

            if (resultObjX86 != null && resultObjX86.ToString() != "default")
            {
                return resultObjX86.ToString();
            }
        }
        catch (Exception)
        {
        }

        return null;
    }
}