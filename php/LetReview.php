<?php
$_fp = fopen("php://stdin", "r");
/* Enter your code here. Read input from STDIN. Print output to STDOUT */
fscanf($_fp, "%d", $t);
for($i = 0; $i < $t; $i++) {
    fscanf($_fp, "%s", $s);
    $arr = str_split($s);
    $even = "";
    $odd = "";
    for($j = 0; $j < count($arr); $j++) {
        if ($j % 2 == 0) {
            $even = $even.$arr[$j];
        }
        else {
            $odd = $odd.$arr[$j];
        }
    }
    echo $even." ".$odd."\n";
}
?>
