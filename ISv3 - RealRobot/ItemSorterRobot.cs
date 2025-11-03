using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace InventorySystem;

public class ItemSorterRobot(string ipAddress = "localhost") : Robot(ipAddress)
{
    public const string TemplateFileName = "move-items-to-shipment-box.script";
    public string Program = File.ReadAllText(TemplateFileName) + System.Environment.NewLine;

    public async Task PickUp(uint itemLocation)
    {
        // Vi forstår at URScript-sessionen forventer ét program ad gangen.
        SendUrscript(string.Format(CultureInfo.InvariantCulture, Program, itemLocation));
        do { await Task.Delay(100); } while (ProgramRunning);
    }
}