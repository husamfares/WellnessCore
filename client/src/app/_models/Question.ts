export interface Question {
  id: number;
  caption: string;
  askedBy: string;
  createdAt: string;
  answers: Answer[];
}
  
  export interface Answer {
    id: number;
    answerText: string;
    answeredBy: string;
    createdAt: string;
  }
  