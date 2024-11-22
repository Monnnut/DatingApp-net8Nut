import { ResolveFn } from '@angular/router';
import { Member } from '../_models/member';
import { MembersService } from '../_services/members.service';
import { inject } from '@angular/core';

export const memberDetailedResolver: ResolveFn<Member | null> = (
  route,
  state
) => {

  //give our routes member data before the page load 
  const memberService = inject(MembersService);

  const username = route.paramMap.get('username');

  if(!username){
    return null;
  }

  console.log('Resolver:', { username, data: memberService.getMember(username) });

  return memberService.getMember(username);
};
