// Declare second integer, double, and String variables.
$number1;
$number2;
$text;
// Read and save an integer, double, and String to your variables.
$number1 = fgets($handle);
$number2 = fgets($handle);
$text = fgets($handle);
// Print the sum of both integer variables on a new line.
echo $i + $number1;
echo "\n";
// Print the sum of the double variables on a new line.
echo number_format($d + $number2, 1, '.', '')."\n";
// Concatenate and print the String variables on a new line
// The 's' variable above should be printed first.
echo $s.$text;
