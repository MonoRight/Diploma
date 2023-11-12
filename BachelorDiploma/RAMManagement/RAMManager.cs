using BachelorDiploma.NotificationManagement;
using Notifications.Wpf;
using System;
using System.Linq;
using System.Management;

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
                Notification.Show("Ємність оперативної пам'яті:", $"Всього: {totalRam} Гб\nЗайнято: {busyRam} Гб\nВільно: {freeRam} Гб\n\nНедостатньо оперативної пам’яті для роботи програми", NotificationType.Error, 0, 0, 15);
            }
            else if (totalRam > 3.5 && freeRam < 2)
            {
                Notification.Show("Ємність оперативної пам'яті:", $"Всього: {totalRam} Гб\nЗайнято: {busyRam} Гб\nВільно: {freeRam} Гб\n\nНедостатньо вільної пам'яті RAM для роботи програми. Закрийте програми, які не використовуються", NotificationType.Warning, 0, 0, 15);
            }
            else
            {
                Notification.Show("Ємність оперативної пам'яті:", $"Всього: {totalRam} Гб\nЗайнято: {busyRam} Гб\nВільно: {freeRam} Гб\n\nОбсяг вільної оперативної пам'яті дозволяє коректно використовувати програму", NotificationType.Success, 0, 0, 15);
            }
        }
    }
}
