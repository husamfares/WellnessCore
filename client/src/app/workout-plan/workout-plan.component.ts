import { CommonModule } from '@angular/common';
import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { Router, RouterLink, RouterModule } from '@angular/router';

@Component({
  selector: 'app-workout-plan',
  templateUrl: './workout-plan.component.html',
  styleUrls: ['./workout-plan.component.css'],
  imports:[RouterLink, CommonModule, RouterModule]
})
export class WorkoutPlanComponent implements OnInit, AfterViewInit, OnDestroy {
  
  private animationInterval: any;

  constructor(private router: Router) { }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    this.initializeAnimations();
  }

  ngOnDestroy(): void {
    if (this.animationInterval) {
      clearInterval(this.animationInterval);
    }
  }

  private initializeAnimations(): void {
    // Add intersection observer for scroll animations
    const observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          entry.target.classList.add('animate-in');
        }
      });
    }, { threshold: 0.1 });

    // Observe all workout cards
    document.querySelectorAll('.workout-card').forEach(card => {
      observer.observe(card);
    });

    // Add dynamic hover effects
    this.startDynamicEffects();
  }

  private startDynamicEffects(): void {
    this.animationInterval = setInterval(() => {
      const cards = document.querySelectorAll('.workout-card');
      cards.forEach((card: any, index) => {
        const delay = index * 200;
        setTimeout(() => {
          card.style.transform += ' scale(1.001)';
          setTimeout(() => {
            card.style.transform = card.style.transform.replace(' scale(1.001)', '');
          }, 100);
        }, delay);
      });
    }, 5000);
  }

  onCardHover(event: MouseEvent): void {
    const card = event.currentTarget as HTMLElement;
    card.style.transition = 'all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275)';
  }

  onCardClick(planType: string, event: MouseEvent): void {
    const card = event.currentTarget as HTMLElement;
    
    // Add selection animation
    card.style.transform = 'scale(0.98)';
    
    setTimeout(() => {
      card.style.transform = '';
    }, 150);

    // Navigate after animation
    setTimeout(() => {
      this.router.navigate([`/${planType}`]);
    }, 300);
  }

  // Scroll to plans section
  scrollToPlans(): void {
    const plansSection = document.querySelector('.plans-section');
    if (plansSection) {
      plansSection.scrollIntoView({ behavior: 'smooth' });
    }
  }
}