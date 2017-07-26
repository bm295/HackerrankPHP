<?php

$handle = fopen ("php://stdin","r");
$arr = array();
for($arr_i = 0; $arr_i < 6; $arr_i++) {
   $arr_temp = fgets($handle);
   $arr[] = explode(" ",$arr_temp);
  array_walk($arr[$arr_i],'intval');
}
$max = 0;
for ($i = 0; $i < 4; $i++) {
    for ($j = 0; $j < 4; $j++) {
        $temp = $arr[$i][$j] + $arr[$i][$j+1] + $arr[$i][$j+2] + $arr[$i+1][$j+1] + $arr[$i+2][$j] + $arr[$i+2][$j+1] + $arr[$i+2][$j+2];
        if ($i == 0 && $j == 0) {
            $max = $temp;
        }
        else {
            if ($temp > $max) {
                $max = $temp;
            }
        }        
    }
}
echo $max;
?>
