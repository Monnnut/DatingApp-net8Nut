import { Directive, inject, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Directive({
  selector: '[appHasRole]', //*appHasRole
  standalone: true
})
export class HasRoleDirective implements OnInit {

 @Input() appHasRole:string[] = [];
 private accountService= inject(AccountService);
 //ViewContainerRef and TemplateRef are powerful utilities used for 
 //creating, managing, and embedding templates dynamically

 // can dynamically create, attach, and manipulate components or templates
 // at runtime within a specific location in your Angular application.
 private viewContainerRef = inject(ViewContainerRef);
 //Used to reference and manipulate a template defined in the Angular HTML
 // file, typically passed to directives or dynamically created views.
 private templateRef = inject(TemplateRef)

 ngOnInit(): void {
  if(this.accountService.role()?.some((r: string) => this.appHasRole.includes(r))){
    this.viewContainerRef.createEmbeddedView(this.templateRef)
  } else{
    this.viewContainerRef.clear();
  }
}

}
