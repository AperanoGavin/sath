import { IAuthService } from '@auth/domain';

export class LoginUseCase {
  constructor(private authService: IAuthService) {}

  execute(): void {
    this.authService.login();
  }
}
