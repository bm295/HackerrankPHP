class Student extends person {
    private $testScores;
  
    /*	
    *   Class Constructor
    *   
    *   Parameters:
    *   firstName - A string denoting the Person's first name.
    *   lastName - A string denoting the Person's last name.
    *   id - An integer denoting the Person's ID number.
    *   scores - An array of integers denoting the Person's test scores.
    */
    // Write your constructor here
    public function __construct($firstName, $lastName, $id, $scores) {
        $this->firstName = $firstName;
        $this->lastName = $lastName;
        $this->id = $id;
        $this->testScores = $scores;
    }

    /*	
    *   Function Name: calculate
    *   Return: A character denoting the grade.
    */
    // Write your function here
    function calculate() {
        $grade;
        $sum = 0;
        foreach($this->testScores as $score) {
            $sum += $score;
        }
        $a = $sum / count($this->testScores);
        if ($a >= 90 && $a <= 100) {
            $grade = "O";
        }
        else if ($a >= 80 && $a < 90) {
            $grade = "E";
        }
        else if ($a >= 70 && $a < 80) {
            $grade = "A";
        }
        else if ($a >= 55 && $a < 70) {
            $grade = "P";
        }
        else if ($a >= 40 && $a < 55) {
            $grade = "D";
        }
        else if ($a < 40) {
            $grade = "T";
        }
        return $grade;
    }
}
