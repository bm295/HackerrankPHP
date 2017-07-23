<?php
$_fp = fopen("php://stdin", "r");
/* Enter your code here. Read input from STDIN. Print output to STDOUT */
fscanf($_fp, "%d", $n);
$arr = array();
for ($i = 0; $i < $n; $i++) {
    $entry = str_replace("\n", "", fgets($_fp));
    $temp = explode(" ", $entry);
    $arr[$temp[0]] = $temp[1];
}

while(($query = fgets($_fp)) != false) {
    $temp = str_replace("\n", "", $query);
    if (isset($arr[$temp])) {
        echo $temp."=".$arr[$temp]."\n";
    }
    else {
        echo "Not found\n";
    }
}
?>
