import { HttpParams, HttpResponse } from '@angular/common/http';
import { PaginatedResult } from '../_models/pagination';
import { signal } from '@angular/core';

export function setPaginatedResponse<T>(
  response: HttpResponse<T>,
  paginatedResultsignal: ReturnType<typeof signal<PaginatedResult<T> | null>>
) {
  paginatedResultsignal.set({
    items: response.body as T, //assertion to get around error
    pagination: JSON.parse(response.headers.get('Pagination')!),
  });
}

export function setPaginationHeaders(pageNumber: number, pageSize: number) {
  let params = new HttpParams();

  if (pageNumber && pageSize) {
    params = params.append('pageNumber', pageNumber);
    params = params.append('pageSize', pageSize);
  }
  return params;
}
