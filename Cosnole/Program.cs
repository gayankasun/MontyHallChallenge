using System;

namespace MontyHall
{
    class Program
    {
        static void Main(string[] args)
        {
            // local variables to hold result and random generator
            Random random = new Random();
            int wins = 0;
            int losses = 0;

            // iterate our MontyHall routine
            for (int i = 0; i < 1000000; i++)
            {
                // changeDoor: 
                // 0 = no, the contestant stays with their initial pick,
                // after the offer to switch after
                // the disclosure of a "Goat" door
                // 1 = yes, the contestant chose to switch doors after
                // the disclosure of a "Goat" door
                int changeDoor = 0;
                //int changeDoor = 1;

                // calculate whether or not the contestant wins the Car -
                // random pickedDoor: 0, 1 or 2 for the door
                // the contestant initially picked
                // changeDoor: 0 = no, 1 = yes. The contentment decides
                // to change their selection after disclosure of a "Goat" door
                // random carDoor: 0, 1 or 2 for the door containing the car
                // random goatDoorToRemove: 0 = leftmost Goat door,
                // 1 = rightmost Goat door. Monty discloses
                // one incorrect door, this value indicates which one.
                bool result = MontyHallPick(random.Next(3), changeDoor,
                                            random.Next(3), random.Next(1));

                if (result)
                    wins++;
                else
                    losses++;
            }

            Console.WriteLine("Wins: {0} Losses: {1}  Total: {2}",
                              wins, losses, wins + losses);
            Console.ReadLine();
        }

        public static bool MontyHallPick(int pickedDoor, int changeDoor,
                                      int carDoor, int goatDoorToRemove)
        {
            bool win = false;

            // randomly remove one of the *goat* doors,
            // but not the "contestants picked" ONE!
            int leftGoat = 0;
            int rightGoat = 2;
            switch (pickedDoor)
            {
                case 0: leftGoat = 1; rightGoat = 2; break;
                case 1: leftGoat = 0; rightGoat = 2; break;
                case 2: leftGoat = 0; rightGoat = 1; break;
            }

            int keepGoat = goatDoorToRemove == 0 ? rightGoat : leftGoat;

            // would the contestant win with the switch or the stay?
            if (changeDoor == 0)
            {
                // not changing the initially picked door
                win = carDoor == pickedDoor;
            }
            else
            {
                // changing picked door to the other door remaining
                win = carDoor != keepGoat;
            }

            return win;
        }
    }
}