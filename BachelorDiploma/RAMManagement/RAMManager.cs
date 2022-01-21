using BachelorDiploma.NotificationManagement;
using System;
using System.Linq;
using System.Management;
using Notifications.Wpf;

namespace BachelorDiploma.RAMManagement
{
    public static class RAMManager
    {
        public static void ShowNotificationRAMInformation()
        {
            double totalRam = 0, busyRam = 0, freeRam = 0;
            ManagementObjectSearcher ramMonitor = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize,FreePhysicalMemory FROM Win32_OperatingSystem");

            foreach (ManagementObject objram in ramMonitor.Get().OfType<ManagementObject>())
            {
                totalRam = Convert.ToDouble(objram["TotalVisibleMemorySize"]);
                freeRam = Convert.ToDouble(objram["FreePhysicalMemory"]);
                busyRam = totalRam - freeRam;
            }

            totalRam = Math.Round(totalRam *= Math.Pow(1024, -2), 2);
            busyRam = Math.Round(busyRam *= Math.Pow(1024, -2), 2);
            freeRam = Math.Round(freeRam *= Math.Pow(1024, -2), 2);

            if (totalRam <= 3.5)
            {
                Notification.Show("Capacity of the RAM:", $"Total: {totalRam} GB\nBusy: {busyRam} GB\nFree: {freeRam} GB\n\nNot enough total RAM memory for the program to run", NotificationType.Error, 0, 0, 15);
            }
            else if(totalRam > 3.5 && freeRam < 2)
            {
                Notification.Show("Capacity of the RAM:", $"Total: {totalRam} GB\nBusy: {busyRam} GB\nFree: {freeRam} GB\n\nNot enough free RAM memory for the program to run. Please close unused applications", NotificationType.Warning, 0, 0, 15);
            }
            else
            {
                Notification.Show("Capacity of the RAM:", $"Total: {totalRam} GB\nBusy: {busyRam} GB\nFree: {freeRam} GB\n\nThe amount of free RAM memory makes possible to use the application correctly", NotificationType.Success, 0, 0, 15);
            }
        }
    }
}
