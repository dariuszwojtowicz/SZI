using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Kelner.Model
{
    public partial class WaiterWorker
    {
        private int CheckFreeTables()
        {
            var random = new Random();
            return random.Next(1, 4);
        }

        private int CheckDirtyTables()
        {
            int dirtyCount = 0;
            Console.WriteLine("Sprawdzam stan stolików:");
            foreach (var tableWorker in tableWorkers)
	        {
                BitmapImage newImage;
		        switch (tableWorker.Number)
                {
                    case 1:
                        newImage = this.TableImage1;
                        break;
                    case 2:
                        newImage = this.TableImage2;
                        break;
                    case 3:
                        newImage = this.TableImage3;
                        break;
                    case 4:
                        newImage = this.TableImage4;
                        break;
                    case 5:
                        newImage = this.TableImage5;
                        break;
                    default:
                        newImage = new BitmapImage();
                        break;
                }
                String readLetter = neuralBackProp.ReadLetter(newImage);
                if (readLetter == "B")
                {
                    Console.WriteLine("Stolik nr.{0} jest brudny.", tableWorker.Number);
                    dirtyCount++; 
                }
                else
                {
                    Console.WriteLine("Stolik nr.{0} jest czysty.", tableWorker.Number);
                }
	        }
            return dirtyCount;
        }

        private int WaitingTime()
        {
            var random = new Random();
            return random.Next(0, 60);
        }

        private int OrdersOnKitchen()
        {
            var random = new Random();
            return random.Next(0, 5);
        }
    }
}
