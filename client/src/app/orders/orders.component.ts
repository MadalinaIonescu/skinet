import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import { Component, OnInit } from '@angular/core';
import { IOrder } from '../shared/models/order';
import { OrdersService } from './orders.service';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss']
})
export class OrdersComponent implements OnInit {
orders: IOrder[]
  constructor(private ordersService: OrdersService) { }

  ngOnInit(): void {
    this.getOrders();
  }

  getOrders(){
    this.ordersService.getOrders().subscribe( {
      next: (orders:IOrder[]) => {
      this.orders = orders.sort((a,b) => a.id - b.id)
    }, error:(e) =>{
      console.log(e);
    }});
  }

}
