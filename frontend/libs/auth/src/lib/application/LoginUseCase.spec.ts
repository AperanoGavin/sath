import { LoginUseCase } from './LoginUseCase';
import { IAuthService } from '../domain/IAuthService';

describe('LoginUseCase', () => {
  it('devrait appeler authService.login()', () => {
    const mockAuth: IAuthService = {
      login: jest.fn(),
      handleCallback: jest.fn(),
      logout: jest.fn(),
      getCurrentUser: jest.fn(),
    };
    const uc = new LoginUseCase(mockAuth);
    uc.execute();
    expect(mockAuth.login).toHaveBeenCalled();
  });
});
