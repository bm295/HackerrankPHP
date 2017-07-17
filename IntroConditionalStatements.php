<?php

$handle = fopen ("php://stdin","r");
fscanf($handle,"%d",$N);
if ($N % 2 == 1) {
    echo "Weird";
}
else if ($N >= 2 && $N <= 5) {
    echo "Not Weird";
}
else if ($N >= 6 && $N <= 20) {
    echo "Weird";
}
else if ($N > 20) {
    echo "Not Weird";
}
?>
