export interface TrainerSubscription {
  id: number;
  type: string;
  title: string;
  price: number;
}

export interface TherapistSessionPrice {
  id: number;
  title: string;
  price: number;
}

export interface Profile {

  id: number; 
  username: string;
  role: string;
  mobileNumber: string;
  location: string;
  gymName: string;
  clinicName: string;
  profilePictureUrl: string;
  subscriptions: TrainerSubscription[];
  sessionPrices: TherapistSessionPrice[];
}
