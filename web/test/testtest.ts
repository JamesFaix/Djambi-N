import { expect } from 'chai';

describe('Thing', () => {
    it('is a thing', () => {
        expect(1).to.equal(1);
    })

    it('is not a thing', () => {
        expect(1).to.equal(2);
    })
})