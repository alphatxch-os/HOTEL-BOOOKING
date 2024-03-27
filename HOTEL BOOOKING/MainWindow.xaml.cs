using System;
using System.Windows;
using MySql.Data.MySqlClient;

namespace HOTEL_BOOOKING
{
    public partial class MainWindow : Window
    {
        string connStr = "server=ND-COMPSCI;" +
               "username=TL_S2201761;" +
               "database=TL_S2201761_hotel;" +
               "port=3306;" +
               "password=Notre021205";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BookRoom_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve input data
            // Converting null literal or possible null value to non-nullable type.
            string checkInDate = dpCheckInDate.SelectedDate?.ToString("yyyy-MM-dd");
// Converting null literal or possible null value to non-nullable type.
 // Converting null literal or possible null value to non-nullable type.
            string checkOutDate = dpCheckOutDate.SelectedDate?.ToString("yyyy-MM-dd");
// Converting null literal or possible null value to non-nullable type.
            string customerName = txtCustomerName.Text;
            string roomNumber = cmbRoomNumber.SelectedValue?.ToString(); // Read selected value from ComboBox

            // Validate input data (you can add more validation as needed)
            if (string.IsNullOrEmpty(checkInDate) || string.IsNullOrEmpty(checkOutDate) ||
                string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(roomNumber))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            // Validate check-out date is after check-in date
            if (DateTime.Parse(checkOutDate) <= DateTime.Parse(checkInDate))
            {
                MessageBox.Show("Check-out date must be after check-in date.");
                return;
            }

            // Connect to the database
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    // Check if there's already a booking for the specified room and dates
                    string query = "SELECT COUNT(*) FROM Bookings " +
                                   "WHERE RoomNumber = @RoomNumber " +
                                   "AND ((CheckInDate BETWEEN @CheckInDate AND @CheckOutDate) OR " +
                                   "(CheckOutDate BETWEEN @CheckInDate AND @CheckOutDate))";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@RoomNumber", roomNumber);
                    cmd.Parameters.AddWithValue("@CheckInDate", checkInDate);
                    cmd.Parameters.AddWithValue("@CheckOutDate", checkOutDate);

                    int existingBookingsCount = Convert.ToInt32(cmd.ExecuteScalar());

                    if (existingBookingsCount > 0)
                    {
                        MessageBox.Show("There is already a booking for this room during the specified dates.");
                    }
                    else
                    {
                        // Insert booking details into the database
                        string insertQuery = "INSERT INTO Bookings (CheckInDate, CheckOutDate, CustomerName, RoomNumber) " +
                                             "VALUES (@CheckInDate, @CheckOutDate, @CustomerName, @RoomNumber)";
                        MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                        insertCmd.Parameters.AddWithValue("@CheckInDate", checkInDate);
                        insertCmd.Parameters.AddWithValue("@CheckOutDate", checkOutDate);
                        insertCmd.Parameters.AddWithValue("@CustomerName", customerName);
                        insertCmd.Parameters.AddWithValue("@RoomNumber", roomNumber);

                        int rowsAffected = insertCmd.ExecuteNonQuery();
                        // extra validation 
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Booking has been confirmed ! !");
                        }
                        else
                        {
                            MessageBox.Show("Booking failed. Please try again.");
                        }
                    }
                }
                catch (Exception ex)
                //error handling 
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
        }

        private void CheckAvailability_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve input data
            string checkInDate = dpCheckInDate.SelectedDate?.ToString("yyyy-MM-dd");
            string checkOutDate = dpCheckOutDate.SelectedDate?.ToString("yyyy-MM-dd");
            string roomNumber = cmbRoomNumber.SelectedValue?.ToString(); // Read selected value from ComboBox

            // Validate input data (you can add more validation as needed)
            if (string.IsNullOrEmpty(checkInDate) || string.IsNullOrEmpty(checkOutDate) || string.IsNullOrEmpty(roomNumber))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            // Validate check-out date is after check-in date
            if (DateTime.Parse(checkOutDate) <= DateTime.Parse(checkInDate))
            {
                MessageBox.Show("Check-out date must be after check-in date.");
                return;
            }

            // Connect to the database
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    // Check if there's already a booking for the specified room and dates
                    string query = "SELECT COUNT(*) FROM Bookings " +
                                   "WHERE RoomNumber = @RoomNumber " +
                                   "AND ((CheckInDate BETWEEN @CheckInDate AND @CheckOutDate) OR " +
                                   "(CheckOutDate BETWEEN @CheckInDate AND @CheckOutDate))";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@RoomNumber", roomNumber);
                    cmd.Parameters.AddWithValue("@CheckInDate", checkInDate);
                    cmd.Parameters.AddWithValue("@CheckOutDate", checkOutDate);

                    int existingBookingsCount = Convert.ToInt32(cmd.ExecuteScalar());

                    if (existingBookingsCount > 0)
                    { // to make sure previous bookings dont get overwritten etc

                        MessageBox.Show("The room is not available for the specified dates.");
                    }
                    else
                    {
                        MessageBox.Show("The room is available for the specified dates.");
                    }
                } // error handling 
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
        }

        private void CancelBooking(object sender, RoutedEventArgs e)
        {
            // Retrieve input data
            string checkInDate = dpCheckInDate.SelectedDate?.ToString("yyyy-MM-dd");
            string checkOutDate = dpCheckOutDate.SelectedDate?.ToString("yyyy-MM-dd");
            string roomNumber = cmbRoomNumber.SelectedValue?.ToString(); // Read selected value from ComboBox

            // Validate input data (you can add more validation as needed)
            if (string.IsNullOrEmpty(checkInDate) || string.IsNullOrEmpty(checkOutDate) || string.IsNullOrEmpty(roomNumber))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            // Prompt the user to confirm the cancellation
            MessageBoxResult result = MessageBox.Show("Are you sure you want to cancel this booking?", "Cancel Booking", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            // Connect to the database
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    // Execute an SQL query to delete the booking from the database
                    string query = "DELETE FROM Bookings " +
                                   "WHERE RoomNumber = @RoomNumber " +
                                   "AND CheckInDate = @CheckInDate " +
                                   "AND CheckOutDate = @CheckOutDate";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
// adding values 
                    cmd.Parameters.AddWithValue("@RoomNumber", roomNumber);
                    cmd.Parameters.AddWithValue("@CheckInDate", checkInDate);
                    cmd.Parameters.AddWithValue("@CheckOutDate", checkOutDate);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    // validation 
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Booking canceled successfully!");
                    }
                    else
                    {
                        MessageBox.Show("No booking found for the specified details.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
        }

        private void ModifyBooking(object sender, RoutedEventArgs e)
        {
            // Retrieve input data
            string roomNumber = cmbRoomNumber.SelectedValue?.ToString(); // Read selected value from ComboBox
            string checkInDate = dpCheckInDate.SelectedDate?.ToString("yyyy-MM-dd");
            string checkOutDate = dpCheckOutDate.SelectedDate?.ToString("yyyy-MM-dd");
            string customerName = txtCustomerName.Text;

            // Validate check-out date is after check-in date
            if (DateTime.Parse(checkOutDate) <= DateTime.Parse(checkInDate))
            {
                MessageBox.Show("Check-out date must be after check-in date.");
                return;
            }

            // Validate input data (you can add more validation as needed)
            if (string.IsNullOrEmpty(roomNumber))
            {
                MessageBox.Show("Please enter a room number.");
                return;
            }
        }
    }
}
