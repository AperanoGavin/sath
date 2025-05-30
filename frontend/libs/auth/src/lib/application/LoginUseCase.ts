import { IAuthService } from '../domain/IAuthService';

export class LoginUseCase {
  constructor(private authService: IAuthService) {}

  execute(): void {
    this.authService.login();
  }
}
