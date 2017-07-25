<?php

$handle = fopen ("php://stdin","r");
fscanf($handle,"%d",$n);
$binArr = str_split(decbin($n));
$count = 0;
$max = 0;
foreach($binArr as $num) {
    if ($num == "1") {
        $count++;
        if ($count > $max) {
            $max = $count;
        }
    }
    else {
        $count = 0;
    }    
}
echo $max;
?>
