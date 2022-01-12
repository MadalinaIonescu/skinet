import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IOrder } from 'src/app/shared/models/order';
import { BreadcrumbService } from 'xng-breadcrumb';
import { OrdersService } from '../orders.service';

@Component({
  selector: 'app-order-detailed',
  templateUrl: './order-detailed.component.html',
  styleUrls: ['./order-detailed.component.scss']
})
export class OrderDetailedComponent implements OnInit {
order: IOrder;

  constructor(private ordersService: OrdersService, private route: ActivatedRoute,
     private breadCrumbService: BreadcrumbService) {
       this.breadCrumbService.set('@OrderDetailed', '');
      }

  ngOnInit(): void {
    this.getOrderDetailed(+this.route.snapshot.paramMap.get('id'));
  }

  getOrderDetailed(id: number){
    this.ordersService.getOrderDetailed(id).subscribe({
      next: (order: IOrder) =>{
      this.order = order;
      this.breadCrumbService.set('@OrderDetailed', `Order# ${order.id} - ${order.status}`);
    }, error:(e) =>{
      console.log(e);
    }})
  }

}
