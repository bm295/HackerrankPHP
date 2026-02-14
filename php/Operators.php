<?php
$handle = fopen("php://stdin","r");
$cost = fgets($handle);
$tip = fgets($handle);
$tax = fgets($handle);
$total = $cost + $cost*$tip/100 + $cost*$tax/100;
echo "The total meal cost is ".round($total)." dollars.";
fclose($handle);
?>
