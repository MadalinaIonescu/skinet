import { Component, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { BasketService } from 'src/app/basket/basket.service';
import { IDeliveryMethod } from 'src/app/shared/models/deliveryMethod';
import { CheckoutService } from '../checkout.service';

@Component({
  selector: 'app-checkout-delivery',
  templateUrl: './checkout-delivery.component.html',
  styleUrls: ['./checkout-delivery.component.scss']
})
export class CheckoutDeliveryComponent implements OnInit {
  @Input() checkoutForm : FormGroup;
  @Input() isCompleted: boolean;
  deliveryMethods : IDeliveryMethod[]
  //selectedDeliveryMethod : IDeliveryMethod;
  
  constructor(private checkoutService: CheckoutService, private basketService: BasketService) { }

  ngOnInit(): void {
    this.checkoutService.getDeliveryMethods().subscribe((dm : IDeliveryMethod[]) =>{
      this.deliveryMethods = dm;
    }, error => {
      console.log(error);
    });
  /*  this.selectedDeliveryMethod = this.basketService.selectedDeliveryMethod;
    if(this.selectedDeliveryMethod){
      console.log(this.selectedDeliveryMethod.description + this.selectedDeliveryMethod.id);
    }*/
  }

  setShippingPrice(deliveryMethod: IDeliveryMethod){
    this.basketService.setShippingPrice(deliveryMethod);
  }

}
